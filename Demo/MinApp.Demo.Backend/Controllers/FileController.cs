using MinApp.Actions;
using MinApp.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinApp.Demo.Backend.Controllers
{

    public class FileController : ApiController
    {

        [Route("file/read")]
        public IActionResult LoadFile(string filePath)
        {
            return this.File(filePath, FileActionResult.ApplicationOctetStreamMime);
        }

    }

}
