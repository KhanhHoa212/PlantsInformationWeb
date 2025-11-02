using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace PlantsInformationWeb.Pages.Shared
{
    public class _Pagination : PageModel
    {
        private readonly ILogger<_Pagination> _logger;

        public _Pagination(ILogger<_Pagination> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}