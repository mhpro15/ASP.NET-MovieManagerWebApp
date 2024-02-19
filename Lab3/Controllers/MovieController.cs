using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Lab3.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;

namespace Lab3.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MovieController : Controller
	{
		private readonly IDynamoDBContext _context;
		public Movie Movie { get; set; }

		private readonly IAmazonS3 _s3Client;

		private readonly IAmazonDynamoDB _dbclient;

		private readonly SignInManager<IdentityUser> _signInManager;


		private string bucketImg = "lab3image";
		private string bucketMovie = "lab3video";

		public MovieController(IDynamoDBContext context, IAmazonS3 s3Client, IAmazonDynamoDB amazonDynamoDBClient, SignInManager<IdentityUser> signInManager)
		{
			_context = context;
			_s3Client = s3Client;
			_dbclient = amazonDynamoDBClient;
			_signInManager = signInManager;
		}

		private Boolean IsUserLoggedIn()
		{
			if (_signInManager.UserManager.GetUserName(User) != null)
				return true;
			else
				return false;
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var movie = await _context.LoadAsync<Movie>(id);
			if (movie == null) return NotFound();
			return Ok(movie);
		}
		[HttpGet]
		public async Task<IActionResult> GetAllMovies()
		{
			var movies = await _context.ScanAsync<Movie>(default).GetRemainingAsync();
			return Ok(movies);
		}


		[HttpPost]
		public async Task<IActionResult> CreateMovie(Movie movieRequest)
		{
			var movie = await _context.LoadAsync<Movie>(movieRequest.Id);
			if (movie != null) return BadRequest($"Movie with id {movieRequest.Id} Already Exists");
			await _context.SaveAsync(movieRequest);
			return Ok(movieRequest);
		}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteMovie(int id)
		{
			var movie = await _context.LoadAsync<Movie>(id);
			if (movie == null) return NotFound();
			await _context.DeleteAsync(movie);
			Convert.ToString(id);
			await _s3Client.DeleteObjectAsync(bucketMovie, id + ".mp4");
			return NoContent();
		}
		[HttpPut]
		public async Task<IActionResult> UpdateMovie(Movie movieRequest)
		{
			var movie = await _context.LoadAsync<Movie>(movieRequest.Id);
			if (movie == null) return NotFound();
			await _context.SaveAsync(movieRequest);
			return Ok(movieRequest);
		}

		[Route("/movie")]
		public IActionResult Index()
		{
			if (_signInManager.UserManager.GetUserName(User) != null)
				ViewData["CurrentUser"] = _signInManager.UserManager.GetUserName(User).ToString();
			else
			{
				return RedirectToAction("Index", "Home");
			}
			return View();
		}

		[Route("/api/movie/user/{username}")]
		[HttpGet]
		public async Task<IActionResult> GetByUser()
		{
			var movies = await _context.ScanAsync<Movie>(new List<ScanCondition> { new ScanCondition("UserId", ScanOperator.Equal, _signInManager.UserManager.GetUserId(User)) }).GetRemainingAsync();
			return Ok(movies);
		}

		[Route("/movie/{id}")]
		[HttpGet]
		public async Task<IActionResult> Watch(string id)
		{
			Trace.WriteLine("id: " + id);
			var movie = await _context.LoadAsync<Movie>(Convert.ToInt32(id));
			if (movie == null) return NotFound();
			ViewData["MovieId"] = id;
			if (_signInManager.UserManager.GetUserName(User) != null)
				ViewData["CurrentUser"] = _signInManager.UserManager.GetUserName(User).ToString();
			//return Ok(movie);
			return View();
		}

		[Route("/movie/edit/{id}")]
		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			if (!IsUserLoggedIn())
				return RedirectToAction("Index", "Login");

			var movie = await _context.LoadAsync<Movie>(Convert.ToInt32(id));
			if (movie == null) return NotFound();
			Trace.WriteLine("id111: " + id);
			ViewData["MovieId"] = id;
			return View();
		}

		[Route("/movie/create")]
		[HttpGet]
		public IActionResult Create()
		{
			if (!IsUserLoggedIn())
				return RedirectToAction("Index", "Login");
			return View();
		}

		public class CreateMovieRequest
		{
			public string Title { get; set; }
			public int ReleaseYear { get; set; }
			public string Director { get; set; }
			public string Genre { get; set; }

			public string ImgUrl { get; set; }
		}

		public CreateMovieRequest MovieRequest { get; set; }
		[Route("/movie/create")]
		[HttpPost]
		public async Task<IActionResult> Create([FromForm] CreateMovieRequest movieRequest)
		{
			try
			{
				int count = 0;
				ScanRequest request = new ScanRequest
				{
					TableName = "Movies",
					Select = Select.COUNT
				};

				var response = await _dbclient.ScanAsync(request);

				if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
				{
					if (response.Count > 0)
					{
						Trace.WriteLine($"There are {response.Count} item(s) in the table");
						count = response.Count + 1;
					}
				}
				Trace.WriteLine("count: " + count);
				Movie movie = new Movie
				{
					Id = count,
					Title = movieRequest.Title,
					ReleaseYear = movieRequest.ReleaseYear,
					Director = movieRequest.Director,
					Genre = movieRequest.Genre,
					ImgUrl = movieRequest.ImgUrl,
					UserId = _signInManager.UserManager.GetUserId(User),
					Rating = 0,
					RateCount = 0,
					Comments = new List<string>()
				};
				Trace.WriteLine("movieRequest: " + movieRequest.Title);
				Trace.WriteLine(Request.Form.Files.Count);

				if (Request.Form.Files.Count > 0)
				{
					IFormFile file = Request.Form.Files["uploadedFile"];
					Trace.WriteLine("file: " + file.FileName);
					var request2 = new PutObjectRequest()
					{
						BucketName = bucketMovie,
						Key = count.ToString() + ".mp4",
						InputStream = file.OpenReadStream()
					};
					request2.Metadata.Add("Content-Type", file.ContentType);
					await _s3Client.PutObjectAsync(request2);
				}
				await _context.SaveAsync(movie);
				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				Trace.WriteLine(e.Message);
				return View("Create");
			}
		}







	}
}
