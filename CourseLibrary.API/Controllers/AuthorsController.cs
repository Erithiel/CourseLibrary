using AutoMapper;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.API.Controllers
{
	[Route("api/authors")]
	[ApiController]
	public class AuthorsController : ControllerBase
	{
		private readonly ICourseLibraryRepository _courseLibraryRepository;
		private readonly IMapper _mapper;


		public AuthorsController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
		{
			_courseLibraryRepository = courseLibraryRepository ??
									   throw new ArgumentNullException(nameof(courseLibraryRepository));
			_mapper = mapper ??
					  throw new ArgumentNullException(nameof(courseLibraryRepository));
		}

		[HttpGet]
		[HttpHead]
		public ActionResult<IEnumerable<AuthorDto>> GetAuthors([FromQuery] string? mainCategory, [FromQuery] string? searchQuery)
		{
			var authorsFromRepo = _courseLibraryRepository.GetAuthors(mainCategory, searchQuery);
			return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo));
		}

		[HttpGet("{authorId}")]
		public IActionResult GetAuthor(Guid authorId)
		{
			var authorFromRepo = _courseLibraryRepository.GetAuthor(authorId);
			if (authorFromRepo == null)
			{
				return NotFound();
			}

			return Ok(_mapper.Map<AuthorDto>(authorFromRepo));
		}

	}
}
