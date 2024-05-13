using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using ManekiApp.Server.Data;

namespace ManekiApp.Server.Controllers
{
    public partial class ExportManekiAppDBController : ExportController
    {
        private readonly ManekiAppDBContext context;
        private readonly ManekiAppDBService service;

        public ExportManekiAppDBController(ManekiAppDBContext context, ManekiAppDBService service)
        {
            this.service = service;
            this.context = context;
        }
    }
}
