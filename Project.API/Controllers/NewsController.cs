using AutoMapper;
using Mediatorium.Model.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Model.Core;
using Project.Model.Model.JsonResponse;
using Project.Model.Model.News;
using Project.Model.Model.Token;
using Project.Model.Model.User;
using Project.Service.AppService;
using Project.Service.ThirdParty;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Project.API.FileConfig;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Project.Model.Model.Configuration;

namespace Project.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {

        private readonly INewsService _newsService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHost;
        private readonly IOptions<ImageConfiguration> _imageConfig;

        public NewsController(INewsService newsService, IMapper mapper, IWebHostEnvironment webHost, IOptions<ImageConfiguration> imageConfig)
        {
            _newsService = newsService;
            _mapper = mapper;
            _webHost = webHost;
            _imageConfig = imageConfig;
        }


        [SwaggerOperation(Summary = "Returns details for selected news")]
        [ProducesResponseType(typeof(NewsJson), 200)]
        [HttpGet("GetNews")]
        public async Task<IActionResult> GetNews(int id)
        {

            // Check if ID is sent or if it doesn't exist
            if (!await _newsService.CheckIfIdExist(id))
            {
                return BadRequest(new ErrorResponse()
                {
                    ErrorMessage = "Invalid ID!",
                    Time = DateTime.Now.ToString()
                });
            }

            News news = await _newsService.GetNews(id);

            // Map News to NewsJson object
            NewsJson newsJson = _mapper.Map<NewsJson>(news);
            newsJson.UserCreated = news.User.Username;
            // Get full path for image
            newsJson.Image = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/{_imageConfig.Value.FolderName}/{news.Image}";

            // Return news details
            return Ok(newsJson);
        }

        [SwaggerOperation(Summary = "Returns list of news")]
        [ProducesResponseType(typeof(List<NewsJson>), 200)]
        [HttpGet("GetAllNews")]
        public async Task<IActionResult> GetAllNews()
        {
            List<News> news = await _newsService.GetNews();
            List<NewsJson> newsList = new List<NewsJson>() { };

            foreach(News item in news)
            {
                // Map News to NewsJson object
                NewsJson newsJson = _mapper.Map<NewsJson>(item);
                newsJson.UserCreated = item.User.Username;
                // Get full path for image
                newsJson.Image = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/{_imageConfig.Value.FolderName}/{item.Image}";
                // Fill NewsJson list
                newsList.Add(newsJson);
            }

            // Return our modified news list
            return Ok(newsList);
        }


        [SwaggerOperation(Summary = "Creates new article(news)")]
        [ProducesResponseType(typeof(SuccessResponse), 200)]
        [HttpPost("CreateNews")]
        public async Task<IActionResult> CreateNews([FromForm] NewsInput newsInput)
        {
            // Check if model is invalid
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse()
                {
                    ErrorMessage = ModelState.First().Value.Errors.First().ErrorMessage,
                    Time = DateTime.Now.ToString()
                });
            }

            // Check if image is uploaded
            if(newsInput.Image == null)
            {
                return BadRequest(new ErrorResponse()
                {
                    ErrorMessage = "Image is required!",
                    Time = DateTime.Now.ToString()
                });
            }

            // Check if logged user is not authorized
            if (!IsUserAuthorized())
            {
                NotAuthorizedResponse();
            }

            // Get logged user RoleId by using his ClaimTypes.Role value
            string roleId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            // If logged user is not admin return unauthorized request
            if(roleId != null && Int32.Parse(roleId) != (int)RoleType.Admin)
            {
                return Unauthorized(new ErrorResponse()
                {
                    ErrorMessage = "Logged user is not authorized for this request!",
                    Time = DateTime.Now.ToString()
                });
            }

            // Create News object and pass NewsInput data into it
            News news = new News();
            news.Title = newsInput.Title;
            news.ShortDescription = newsInput.ShortDescription;
            news.Description = newsInput.Description;

            // Get admin ID by using his ClaimTypes.NameIdentifier value
            string userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            news.UserId = userId != null ? Int32.Parse(userId) : (int?)null;

            // Upload image to server and return its unique name
            news.Image = await FileMethods.UploadImage(_webHost, newsInput.Image, _imageConfig.Value.FolderName);
            // Create news in database
            await _newsService.CreateNews(news);

            // Returns success message 
            return Ok(new SuccessResponse()
            {
                SuccessMessage = "News successfully created!",
                Time = DateTime.Now.ToString()
            });
          
        }

        [SwaggerOperation(Summary = "Updates selected article(news)")]
        [ProducesResponseType(typeof(SuccessResponse), 200)]
        [HttpPut("EditNews")]
        public async Task<IActionResult> EditNews([FromForm] NewsInputEdit newsInput)
        {
            // Check if logged user is not authorized
            if (!IsUserAuthorized())
            {
                NotAuthorizedResponse();
            }

            // Check if ID is sent or if it doesn't exist
            if (!await _newsService.CheckIfIdExist(newsInput.Id))
            {
                return BadRequest(new ErrorResponse()
                {
                    ErrorMessage = "Invalid ID!",
                    Time = DateTime.Now.ToString()
                });
            }

            // Get news
            News news = await _newsService.GetNews(newsInput.Id);
            // Set all non-null values to news object
            news.Title = newsInput.Title ?? news.Title;
            news.ShortDescription = newsInput.ShortDescription ?? news.ShortDescription;
            news.Description = newsInput.Description ?? news.Description;

            // Check if new image is uploaded
            if(newsInput.Image != null)
            {
                // Delete old image from directory
                FileMethods.DeleteFromFolder(_webHost, _imageConfig.Value.FolderName, news.Image);
                // Upload new image
                news.Image = await FileMethods.UploadImage(_webHost, newsInput.Image, _imageConfig.Value.FolderName);
            }

            // Update news object
            await _newsService.EditNews(news);

            // Returns success message 
            return Ok(new SuccessResponse()
            {
                SuccessMessage = "News successfully updated!",
                Time = DateTime.Now.ToString()
            });

        }

        [SwaggerOperation(Summary = "Removes selected article(news)")]
        [ProducesResponseType(typeof(SuccessResponse), 200)]
        [HttpDelete("DeleteNews")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            // Check if logged user is not authorized
            if (!IsUserAuthorized())
            {
                NotAuthorizedResponse();
            }

            // Check if ID is sent or if it doesn't exist
            if (!await _newsService.CheckIfIdExist(id))
            {
                return BadRequest(new ErrorResponse()
                {
                    ErrorMessage = "Invalid ID!",
                    Time = DateTime.Now.ToString()
                });
            }
            News news = await _newsService.GetNews(id);

            // Delete image from directory if it exists
            if (!String.IsNullOrWhiteSpace(news.Image))
            {
                FileMethods.DeleteFromFolder(_webHost, _imageConfig.Value.FolderName, news.Image);
            }

            await _newsService.DeleteNews(news);

            // Returns success message 
            return Ok(new SuccessResponse()
            {
                SuccessMessage = "News successfully removed!",
                Time = DateTime.Now.ToString()
            });
        }

        #region Authorization check methods

        /// <summary>
        /// Returns false if logged user is not authorized for request
        /// </summary>
        /// <returns></returns>
        private bool IsUserAuthorized()
        {
            // Get logged user RoleId by using his ClaimTypes.Role value
            string roleId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            // If logged user is not admin return unauthorized request
            if (roleId != null && Int32.Parse(roleId) != (int)RoleType.Admin)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns Unauthorized response
        /// </summary>
        /// <returns></returns>
        private IActionResult NotAuthorizedResponse()
        {
            return Unauthorized(new ErrorResponse()
            {
                ErrorMessage = "Logged user is not authorized for this request!",
                Time = DateTime.Now.ToString()
            });
        }

        #endregion

    }
}
