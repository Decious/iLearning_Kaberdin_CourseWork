using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KaberdinCourseiLearning.Areas.Collection.Pages
{
    public class CreateModel : PageModel
    {
        private UserManager<CustomUser> userManager;
        private ApplicationDbContext context;
        private ImageManager imageManager;
        public CreateModel(UserManager<CustomUser> userManager, ApplicationDbContext context,ImageManager imageManager)
        {
            this.userManager = userManager;
            this.context = context;
            this.imageManager = imageManager;
        }
        public CustomUser PageUser { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "Collection name")]
            public string Name { get; set; }
            [Display(Name = "Collection theme")]
            public string Theme { get; set; }
            [Display(Description = "Collection Description")]
            public string Description { get; set; }
            public string[] ColumnNames { get; set; }
            public string[] ColumnTypes { get; set; }
            public string PageUserName { get; set; }
        }
        public async Task<IActionResult> OnGetAsync(string name)
        {
            if (name != null)
            {
                if (await isPermitted(name))
                {
                    await LoadReferences();
                    return Page();
                }
                return Forbid();
            }
            return Redirect("~/Index");
        }

        private async Task<bool> isPermitted(string name)
        {
            PageUser = await userManager.FindByNameAsync(name);
            var validator = new UserValidator(userManager);
            return await validator.IsUserValidAsync(PageUser);
        }
        private async Task LoadReferences()
        {
            await context.Entry(PageUser).Collection(i => i.ItemCollections).LoadAsync();
        }
        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            if (await isPermitted(Input.PageUserName) && !String.IsNullOrWhiteSpace(Input.Name))
            {
                var CollectionId = await CreateCollectionAsync(file);
                if (file != null) return new OkObjectResult(CollectionId);
                return RedirectToPage("/Index", new { id = CollectionId });
            }
            return Forbid();
        }
        private async Task<int> CreateCollectionAsync(IFormFile backgroundImage)
        {
            var collectionHelper = new CollectionHelper(context);
            var newCollection = new ProductCollection() { Description = Input.Description, Name = Input.Name, Theme = Input.Theme, UserID = PageUser.Id };
            var collectionID = await collectionHelper.CreateCollectionAsync(newCollection);
            var columns = GetCollectionColumns(collectionID);
            if(columns != null)
                await collectionHelper.AddCollectionColumnsAsync(columns);
            if (backgroundImage != null)
                await imageManager.UploadBackground(backgroundImage, collectionID);
            return collectionID;
        }
        private ProductCollectionColumn[] GetCollectionColumns(int CollectionID)
        {
            if (Input.ColumnNames == null) return null;
            var columns = new List<ProductCollectionColumn>();
            for (int i = 0; i < Input.ColumnNames.Length; i++)
            {
                var column = new ProductCollectionColumn()
                {
                    CollectionID = CollectionID,
                    ColumnName = Input.ColumnNames[i],
                    ColumnType = Input.ColumnTypes[i]
                };
                columns.Add(column);
            }
            return columns.ToArray();
        }
    }
}
