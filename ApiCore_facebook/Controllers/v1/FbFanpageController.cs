﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ApiCore_facebook.ClassController.log;
using ApiCore_facebook.ClassController.v1;
using ApiCore_facebook.Library;
using ApiCore_facebook.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiCore_facebook.Controllers.v1
{
    [Authorize]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [EnableCors("AllowOrigin")]
    public class FbFanpageController : Controller
    {
        db_facebookContext XLDL = new db_facebookContext();
        private IConfiguration configuration;
        private bool ConsoleError = false;
        #region Setting log 
        private readonly ILogRepository _logRepository;
        private readonly ILogger _logger;
        public FbFanpageController(ILogRepository logRepository, ILoggerFactory logger, IConfiguration iconfig)
        {
            _logRepository = logRepository;
            _logger = logger.CreateLogger("ApiCore_facebook.Controllers.FbFanpage");
            configuration = iconfig; //Lấy ra value file appseting.json
            ConsoleError = bool.Parse(configuration.GetSection("ConsoleError").Value);
        }
        #endregion

        /// <summary>
        /// Kiểm tra page đã đã kích hoạt
        /// </summary>
        /// <param name="pram"></param>
        /// <returns></returns>
        [Route("Check_active_page"), HttpGet]
        [ApiExplorerSettings(GroupName = "get")]
        //[ProducesResponseType(typeof(pro_getUsser), 200)]
        //[ProducesResponseType(404)]
        [ResponseCache(Duration = 86400)]
        public async Task<ActionResult> Check_active_page([FromQuery]  Form_FbFanpage.In_check_active_page pram)
        {
            try
            {
                List<object> result = new List<object>();
                var query_user_token = await XLDL.FbPageDetail.AsNoTracking().Select(x => new { x.Quyen, x.IdUser, x.IdPage }).FirstOrDefaultAsync(x => x.IdUser == pram.IdUser && x.IdPage == pram.IdPage);
                var check_page = await XLDL.FbFanpage.AsNoTracking().AnyAsync(x => x.Id == pram.IdPage && x.SubscribedApps == true && x.AccessToken != "");
                if (query_user_token != null && check_page)
                {
                    string _quyen = query_user_token.Quyen.ToString();
                    if (_quyen.Contains("MANAGE"))
                    {
                        _quyen = "admin";
                    }
                    else
                    {
                        _quyen = "employee";
                    }

                    result.Add(new
                    {
                        success = true,
                        quyen = _quyen,
                    });

                }
                else
                {
                    result.Add(new
                    {
                        success = false
                    });
                }
                //_logger.LogInformation(LoggingEvents.GetItem, "Get user (_pro_getUsser)", pram);
                //if (query != null) return Ok(query);

                return Ok(result.FirstOrDefault());

            }
            catch (Exception ex)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, ex, "");
                if (ConsoleError) return NotFound(ex);
                return NotFound("Lỗi ngoại lệ");//401
            }

        }

 /// <summary>
        /// Kiểm tra page đã đã kích hoạt
        /// </summary>
        /// <param name="pram"></param>
        /// <returns></returns>
        [Route("Load_active_page"), HttpGet]
        [ApiExplorerSettings(GroupName = "get")]
        //[ProducesResponseType(typeof(pro_getUsser), 200)]
        //[ProducesResponseType(404)]
        [ResponseCache(Duration = 86400)]
        public async Task<ActionResult> Load_active_page([FromQuery]  Form_FbFanpage.In_check_active_page pram)
        {
            try
            {
                List<object> result = new List<object>();
                var Load_active_page =  from s in XLDL.FbPageDetail
                                        join sa in XLDL.FbFanpage  on s.IdPage equals sa.Id 
                                        where s.IdUser == pram.IdUser
                                        where sa.SubscribedApps == true && sa.AccessToken != "" 
                                        select new
                                       {
                                           id_page = s.IdPage,
                                           quyen = s.Quyen.Contains("MANAGE") ? "admin" : "employee"
                                       };
                await Load_active_page.AsNoTracking().ToListAsync();
                return Ok(Load_active_page);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, ex, "");
                if (ConsoleError) return NotFound(ex);
                return NotFound("Lỗi ngoại lệ");//401
            }

        }


        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
