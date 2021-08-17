using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Hosting;

namespace BeanBag.Controllers
{
    public class TestUploadController : Controller
    {
        private IWebHostEnvironment hostingEnv;

        public TestUploadController(IWebHostEnvironment env)
        {
            this.hostingEnv = env;
        }
        
        public IActionResult Index()
        {

            return View();

        }

        public IActionResult Delete(string filename)
        {
            FileInfo file = new FileInfo(filename);  
            if (file.Exists)//check file exsit or not  
            {  
                file.Delete();  
            }  
            
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)

        {

            var FileDic = "Files";

            string filePath = Path.Combine(hostingEnv.WebRootPath, FileDic);

            if (!Directory.Exists(filePath))

                Directory.CreateDirectory(filePath);

            var fileName = file.FileName;

            var filePath2 = Path.Combine(filePath, fileName);

            using (FileStream fs = System.IO.File.Create(filePath2))

            {

                await file.CopyToAsync(fs);

            }

            return RedirectToAction("Index");

        }
        
      

    }
}

