using BeanBag;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BeanBagIntegrationTests
{
    public class IntegrationTest1 : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        public IntegrationTest1(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;

        }

        //Integration test defined to test if the webcontent pages are being hit when the system is run 
        [Theory]
        [InlineData("/")]
        /*[InlineData("/Home/Home")]
        [InlineData("/Home/Invetory")]
        [InlineData("/Home/Item")]
        [InlineData("/Home/LandingPage")]*/
        public async Task GetHttpRequest(string url)
        {
            //ARRANGE
            var client = _factory.CreateClient();

            //ACT
            var response = await client.GetAsync(url);

            //ASSERT
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }
    }
}
