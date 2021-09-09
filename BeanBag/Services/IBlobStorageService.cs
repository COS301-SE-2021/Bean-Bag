﻿using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeanBag.Services
{
    // This class is an interface for the blob storage service.
    public interface IBlobStorageService
    {
        public Task<string> uploadItemImage(IFormFile file);

        public Task<List<string>> uploadTestImages(IFormFileCollection testImages, string projectId);

        public void deleteTestImageFolder(string projectId);
    }
}
