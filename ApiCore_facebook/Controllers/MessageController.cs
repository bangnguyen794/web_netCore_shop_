﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiCore_facebook.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiCore_facebook.Controllers
{
    [ApiVersion("3")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [EnableCors("AllowOrigin")]
   
    public class MessageController : Controller
    {
        /// <summary>
        /// Bộ nhớ server
        /// </summary>
        private readonly IMemoryCache _cache;
        db_facebook_vmContext context = new db_facebook_vmContext();
        public MessageController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }
        
        [HttpGet]
        public async Task<ActionResult<FbBaoxau>> GetTodoItem()
        {
            var todoItem = await context.FbBaoxau.AsNoTracking().ToListAsync();
            if (todoItem == null)
            {
                return NotFound();
            }
            return Ok(todoItem);//200
        }





        public class from_body_message2
        {
            /// <summary>
            /// Bắt buộc
            /// </summary>
            public string id_page { get; set; }
            /// <summary>
            /// Giá trị phone - id, địa chỉ  ( có hoặc rỗng)
            /// </summary>
            public string key { get; set; }
            /// <summary>
            /// Số item tải lên trong một trang
            /// </summary>
            public int take { get; set; }
            /// <summary>
            /// Trang 1 2 3 ...
            /// </summary>
            public int page_index { get; set; }
        }



        /// <summary>
        /// (Lấy danh sách tin nhắn) 3
        /// </summary>
        /// <remarks>
        /// Ví dụ:
        ///
        ///     POST /Todo
        ///     {
        ///        "id": 1,
        ///        "name": "Item1",
        ///        "isComplete": true
        ///     }
        ///
        /// </remarks>
        /// /// <returns>Oke</returns>
        /// <response code="201">Trả về mục cừa khởi tạo</response>
        /// <response code="400">Nếu trã về rỗng
        /// <code>
        ///  POST /Todo
        ///     {
        ///        "id": 1,
        ///        "name": "Item1",
        ///        "isComplete": true
        ///     }
        /// </code>
        /// </response>    
        [Route("batdongbo"), HttpGet]
        //Time tính bằng giây
        //[Caching_api(Time = 15)]
        //[GzipCompression]
        [ResponseCache(Duration = 100)]
        //Chỉ trả về  2 responses này
        //[ProducesResponseType(201)]
        //[ProducesResponseType(400)]
        public async Task<ActionResult> Lis_message_null([FromQuery] from_body_message2 body)
        {
            try
            {
                //string keyCache = "ListMs";
                //if (!_cache.TryGetValue(keyCache, out IEnumerable<object> query))
                //{

                    var  query = await context.FbMessages.AsNoTracking().AsQueryable().Where(s => s.IdPage == body.id_page).Select(x => new { x.Id, x.IdPage, x.IdUser, x.Message, x.NameUser, x.UpdateTime, x.Views, x.ViewsBy, x.ViewsUpdate, x.Type, x.Phone }).OrderByDescending(x => x.ViewsUpdate).OrderByDescending(x => x.UpdateTime).Skip((body.page_index * body.take) - body.take).Take(body.take).ToListAsync();
                    //MemoryCacher.Add("list_ms_batdongbo", query, DateTimeOffset.UtcNow.AddMinutes(1));

                    //var cacheEntryOptions = new MemoryCacheEntryOptions()
                    //.SetSlidingExpiration(TimeSpan.FromSeconds(30));
                    //_cache.Set(keyCache, query, cacheEntryOptions);
                    return new ObjectResult(query);

                //}
                //else
                //{
                //    return new ObjectResult(_cache.Get(keyCache));
                //}
            }
            catch (Exception Ex)
            {
                return BadRequest(Ex.ToString());
            }
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
