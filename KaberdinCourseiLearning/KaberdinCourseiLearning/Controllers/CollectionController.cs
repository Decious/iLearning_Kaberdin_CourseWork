using CsvHelper;
using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.CollectionRequests;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Helpers;
using KaberdinCourseiLearning.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Controllers
{
    [Route("[controller]/Manage/[action]")]
    public class CollectionController : Controller
    {
        private readonly CollectionManager collectionManager;
        private readonly CustomUserManager userManager;
        private readonly ImageManager imageManager;
        private readonly ApplicationDbContext context;
        private readonly IStringLocalizer<CollectionController> localizer;

        public CollectionController(
            CollectionManager collectionManager,
            CustomUserManager userManager,
            ImageManager imageManager,
            ApplicationDbContext context,
            IStringLocalizer<CollectionController> localizer)
        {
            this.collectionManager = collectionManager;
            this.userManager = userManager;
            this.imageManager = imageManager;
            this.context = context;
            this.localizer = localizer;
        }
        public async Task<IActionResult> Create([FromBody] CreateCollectionRequest request)
        {
            if (!ModelState.IsValid)
                return new JsonResult(new ServerResponse(false, ModelState.GetModelErrors()));
            if (!await userManager.IsUserOwnerOrAdminAsync(User, request.PageUserName)) 
                return new JsonResult(new ServerResponse(false, localizer["NoCreatePermission"], "/User/Profile?name=" + request.PageUserName));
            var response = await collectionManager.CreateCollectionAsync(request);
            return new JsonResult(response);
        }
        public async Task<IActionResult> Edit([FromBody] EditCollectionRequest request) {
            if (!ModelState.IsValid)
                return new JsonResult(new ServerResponse(false, ModelState.GetModelErrors()));
            var collection = await context.ProductCollections
                .Where(c => c.CollectionID == request.CollectionID)
                .Include(c => c.User)
                .FirstOrDefaultAsync();
            if (collection == null) 
                return new JsonResult(new ServerResponse(false, localizer["NoCollectionError"]));
            if (!await userManager.IsUserOwnerOrAdminAsync(User, collection.User.UserName)) 
                return new JsonResult(new ServerResponse(false, localizer["NoEditPermission"], "/Collection?id=" + request.CollectionID));
            var response = await collectionManager.EditCollectionAsync(request);
            return new JsonResult(response);
        }
        public async Task<IActionResult> UpdateImage(IFormFile file, [FromForm] int collectionID)
        {
            var collection = await context.ProductCollections
                .Where(c => c.CollectionID == collectionID)
                .Include(c => c.User)
                .FirstOrDefaultAsync();
            if (collection == null || !await userManager.IsUserOwnerOrAdminAsync(User, collection.User.UserName)) return Forbid();
            await imageManager.UploadBackground(file, collectionID);
            return new OkResult();
        }
        public async Task<IActionResult> Delete(int collectionID)
        {
            var collection = await context.ProductCollections
                .Where(c => c.CollectionID == collectionID)
                .Include(c => c.User)
                .FirstOrDefaultAsync();
            var permitted = await userManager.IsUserOwnerOrAdminAsync(User, collection.User.UserName);
            if (permitted && collection != null)
            {
                context.ProductCollections.Remove(collection);
                await context.SaveChangesAsync();
                return new JsonResult(new ServerResponse(true,localizer["DefaultSuccess"]));
            }
            return new JsonResult(new ServerResponse(false,localizer["DefaultForbidden"]));
        }
        public async Task<IActionResult> Export([FromQuery]int collectionID)
        {
            var collection = await context.ProductCollections
                .Where(c => c.CollectionID == collectionID)
                .Include(c=>c.Columns)
                .Include(c=>c.Products).ThenInclude(p=>p.ColumnValues)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
            if (collection == null)
            {
                var response = new ServerResponse(false, localizer["NoCollectionError"]);
                return new JsonResult(response);
            }
            var rcf = HttpContext.Features.Get<IRequestCultureFeature>();
            var file = GetCSV(collection, rcf.RequestCulture.Culture);
            return File(file, "text/csv", collection.Name+".csv");
        }
        private byte[] GetCSV(ProductCollection collection,CultureInfo culture)
        {
            using (var memory = new MemoryStream())
            {
                using (var writer = new StreamWriter(memory, Encoding.UTF8))
                {
                    using (var csv = new CsvWriter(writer, culture))
                    {
                        csv.WriteField(localizer["Name"]);
                        csv.WriteField(localizer["Tags"]);
                        foreach (var column in collection.Columns.OrderBy(c => c.ColumnID))
                        {
                            csv.WriteField(column.ColumnName);
                        }
                        csv.NextRecord();
                        foreach(var product in collection.Products)
                        {
                            csv.WriteField(product.Name);
                            csv.WriteField(product.Tags);
                            foreach (var value in product.ColumnValues.OrderBy(cv => cv.ColumnID))
                            {
                                csv.WriteField(value.Value);
                            }
                            csv.NextRecord();
                        }
                    }
                }
                return memory.ToArray();
            }
        }
    }
}
