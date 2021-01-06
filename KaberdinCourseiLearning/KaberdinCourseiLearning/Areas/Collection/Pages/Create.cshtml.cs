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
        private SignInManager<CustomUser> signInManager;
        private CustomUser PageUser;
        private IWebHostEnvironment webHostEnvironment;
        public CreateModel(UserManager<CustomUser> userManager, ApplicationDbContext context,SignInManager<CustomUser> signInManager, IWebHostEnvironment webHostEnvironment)
        {
            this.userManager = userManager;
            this.context = context;
            this.signInManager = signInManager;
            this.webHostEnvironment = webHostEnvironment;
        }
        [TempData]
        public string StatusMessage { get; set; }
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
        }
        public async Task<IActionResult> OnGetAsync()
        {
            if(signInManager.IsSignedIn(User))
            {
                if (await isPermitted())
                {
                    await LoadReferences();
                    return Page();
                }
            }
            return Forbid();
        }

        private async Task<bool> isPermitted()
        {
            PageUser = await userManager.GetUserAsync(User);
            var validator = new UserValidator(userManager);
            return await validator.IsUserValidAsync(PageUser);
        }
        private async Task LoadReferences()
        {
            await context.Entry(PageUser).Collection(i => i.ItemCollections).LoadAsync();
        }
        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            if (signInManager.IsSignedIn(User))
            {
                if (await isPermitted() && !String.IsNullOrWhiteSpace(Input.Name))
                {
                    var CollectionId = await CreateCollectionAsync(file);
                    if (file != null) return new OkObjectResult(CollectionId);
                    return RedirectToPage("/Index", new { id = CollectionId });
                }
            }
            return Forbid();
        }
        private async Task<int> CreateCollectionAsync(IFormFile backgroundImage)
        {
            var bgPath = Path.Combine(webHostEnvironment.WebRootPath, "images", "Collection", "Background");
            var collection = await SaveCollectionData();
            var columns = GetCollectionColumns(collection.CollectionID);
            if(columns != null)
            {
                context.AddRange(columns);
                await context.SaveChangesAsync();
            }
            await TrySaveFormFileAsync($"{bgPath}\\{collection.CollectionID}.png", backgroundImage);
            return collection.CollectionID;
        }
        private async Task<ProductCollection> SaveCollectionData()
        {
            var newCollection = new ProductCollection() { Description = Input.Description, Name = Input.Name, Theme = Input.Theme, UserID = PageUser.Id };
            context.Add(newCollection);
            await context.SaveChangesAsync();
            return newCollection;
        }
        private List<ProductCollectionColumn> GetCollectionColumns(int CollectionID)
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
            return columns;
        }
        private async Task TrySaveFormFileAsync(string path, IFormFile file)
        {
            if (file == null) return;
            try
            {
                if (file.Length > 0)
                {
                    using (var stream = System.IO.File.Create(path))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[COLLECTION]Exception during file upload.\n {e.Message}");
            }
        }
    }
}
