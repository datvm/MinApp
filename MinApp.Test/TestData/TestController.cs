using MinApp.Actions;
using MinApp.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinApp.Test.TestData
{

    public class TestController : ApiController
    {

        [Route("test/hello-world")]
        public IActionResult TestHelloWorld()
        {
            return this.String("Hello World");
        }

        [Route("test/params")]
        public IActionResult TestParams(string content, int id, decimal value)
        {
            return this.String($"{content} {id} {value}");
        }

        [Route("test/void")]
        public void TestVoid()
        {

        }

        [Route("test/async")]
        public async Task<IActionResult> TestAsync(int id)
        {
            var result = -1;

            await Task.Run(() =>
            {
                result = id;
            });

            return this.String(result);
        }

        [Route("test/async-empty")]
        public async Task TestAsyncEmpty()
        {
            await Task.Run(() =>
            {

            });
        }

        [Route("test/file")]
        public IActionResult TestFile(string fileName)
        {
            return this.File(fileName);
        }

        [Route("test/json")]
        public async Task<IActionResult> TestJson(string content)
        {
            return await Task.Run(() =>
            {
                return this.Json(new
                {
                    Content = content,
                });
            });
        }

        [HttpGet, Route("test/http-verb")]
        public IActionResult TestHttpVerb(string content)
        {
            return this.String(content);
        }

        [HttpGet, Route("test/http-wildcard-verb")]
        public void TestWildcardHttpVerb1() { }

        [Route("test/http-wildcard-verb")]
        public IActionResult TestWildcardHttpVerb2(string content)
        {
            return this.String(content);
        }

        [HttpPost, Route("test/body")]
        public async Task<IActionResult> TestBody()
        {
            var content = await this.GetBodyString();
            return this.String(content);
        }

        [HttpPost, Route("test/body-json")]
        public async Task<IActionResult> TestBodyJson()
        {
            var content = await this.GetJsonBody<dynamic>();
            return this.Json(content);
        }

    }

}
