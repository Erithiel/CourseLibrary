﻿using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Halpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CourseLibrary.API.Controllers
{
	[ApiController]
	[Route("api/authors")]
	public class AuthorsController : ControllerBase
	{
		private readonly ICourseLibraryRepository _courseLibraryRepository;
		private readonly IMapper _mapper;
		private readonly IPropertyMappingService _propertyMappingService;

		public AuthorsController(ICourseLibraryRepository courseLibraryRepository,
			IMapper mapper, IPropertyMappingService propertyMappingService)
		{
			_courseLibraryRepository = courseLibraryRepository ??
				throw new ArgumentNullException(nameof(courseLibraryRepository));
			_mapper = mapper ??
				throw new ArgumentNullException(nameof(mapper));
			_propertyMappingService = propertyMappingService;
		}

		[HttpGet(Name = "GetAuthors")]
		[HttpHead]
		public IActionResult GetAuthors(
			[FromQuery] AuthorsResourceParameters authorsResourceParameters)
		{
			if (!_propertyMappingService.ValidMappingExistsFor<AuthorDto, Author>(authorsResourceParameters.OrderBy))
			{
				return BadRequest();
			}



			var authorsFromRepo = _courseLibraryRepository.GetAuthors(authorsResourceParameters);

			var previousPageLink = authorsFromRepo.HasPrevious ?
				CreateAuthorsResourceUri(authorsResourceParameters,
				ResourceUriType.PreviousPage) : null;

			var nextPageLink = authorsFromRepo.HasNext ?
				CreateAuthorsResourceUri(authorsResourceParameters,
				ResourceUriType.NextPage) : null;

			var paginationMetadata = new
			{
				totalCount = authorsFromRepo.TotalCount,
				pageSize = authorsFromRepo.PageSize,
				currentPage = authorsFromRepo.CurrentPage,
				totalPages = authorsFromRepo.TotalPages,
				previousPageLink,
				nextPageLink


			};



			Response.Headers.Add("X-Pagination",
				JsonSerializer.Serialize(paginationMetadata));

			return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo).ShapeData(authorsResourceParameters.Fields));
		}

		[HttpGet("{authorId}", Name = "GetAuthor")]
		public IActionResult GetAuthor(Guid authorId, string? fields)
		{
			var authorFromRepo = _courseLibraryRepository.GetAuthor(authorId);

			if (authorFromRepo == null)
			{
				return NotFound();
			}

			return Ok(_mapper.Map<AuthorDto>(authorFromRepo).ShapeData(fields));
		}

		[HttpPost]
		public ActionResult<AuthorDto> CreateAuthor(AuthorForCreationDto author)
		{
			var authorEntity = _mapper.Map<Entities.Author>(author);
			_courseLibraryRepository.AddAuthor(authorEntity);
			_courseLibraryRepository.Save();

			var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);
			return CreatedAtRoute("GetAuthor",
				new { authorId = authorToReturn.Id },
				authorToReturn);
		}

		[HttpOptions]
		public IActionResult GetAuthorsOptions()
		{
			Response.Headers.Add("Allow", "GET,OPTIONS,POST");
			return Ok();
		}

		[HttpDelete("{authorId}")]
		public ActionResult DeleteAuthor(Guid authorId)
		{
			var authorFromRepo = _courseLibraryRepository.GetAuthor(authorId);

			if (authorFromRepo == null)
			{
				return NotFound();
			}

			_courseLibraryRepository.DeleteAuthor(authorFromRepo);

			_courseLibraryRepository.Save();

			return NoContent();
		}

		private string CreateAuthorsResourceUri(
		   AuthorsResourceParameters authorsResourceParameters,
		   ResourceUriType type)
		{
			switch (type)
			{
				case ResourceUriType.PreviousPage:
					return Url.Link("GetAuthors",
					  new
					  {
						  fields = authorsResourceParameters.Fields,
						  orederBy = authorsResourceParameters.OrderBy,
						  pageNumber = authorsResourceParameters.PageNumber - 1,
						  pageSize = authorsResourceParameters.PageSize,
						  mainCategory = authorsResourceParameters.MainCategory,
						  searchQuery = authorsResourceParameters.SearchQuery
					  });
				case ResourceUriType.NextPage:
					return Url.Link("GetAuthors",
					  new
					  {
						  fields = authorsResourceParameters.Fields,
						  orederBy = authorsResourceParameters.OrderBy,
						  pageNumber = authorsResourceParameters.PageNumber + 1,
						  pageSize = authorsResourceParameters.PageSize,
						  mainCategory = authorsResourceParameters.MainCategory,
						  searchQuery = authorsResourceParameters.SearchQuery
					  });

				default:
					return Url.Link("GetAuthors",
					new
					{
						fields = authorsResourceParameters.Fields,
						orederBy = authorsResourceParameters.OrderBy,
						pageNumber = authorsResourceParameters.PageNumber,
						pageSize = authorsResourceParameters.PageSize,
						mainCategory = authorsResourceParameters.MainCategory,
						searchQuery = authorsResourceParameters.SearchQuery
					});
			}

		}
	}
}
