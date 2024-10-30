using AuthorsWebApi.DTOs;
using AuthorsWebApi.Entities;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace AuthorsWebApi.Utils
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles() {
            CreateMap<AuthorCreationDTO, Author>();

            CreateMap<Author, AuthorDTO>();
            CreateMap<Author, AuthorWithBooksDTO>()
                .ForMember(authorDTO => authorDTO.Books, options => options.MapFrom(MapAuthorBooks));

            CreateMap<BookCreationDTO, Book>()
                .ForMember(book => book.BookAuthor, options => options.MapFrom(MapBookAuthor));

            CreateMap<Book, BookDTO>().ReverseMap();
            CreateMap<Book, BookWithAuthorsDTO>()
                .ForMember(bookDTO => bookDTO.Authors, options => options.MapFrom(MapBookAuthors));
            CreateMap<Book, BookPatchDTO>().ReverseMap();
            CreateMap<CommentCreationDTO, Comment>();
            CreateMap<Comment, CommentDTO>();
            CreateMap<Comment, CommentWithBookDTO>();
        }

        private List<BookDTO> MapAuthorBooks(Author author, AuthorWithBooksDTO authorWithBooksDTO)
        {
            List<BookDTO> result = new List<BookDTO>();

            if(author.BookAuthor == null) { return  result; }

            foreach (BookAuthor bookAuthor in author.BookAuthor)
            {
                result.Add(new BookDTO
                {
                    Id = bookAuthor.BookId,
                    Title = bookAuthor.Book.Title
                });
            }

            return result;
        }

        private List<AuthorDTO> MapBookAuthors (Book book, BookWithAuthorsDTO bookWithAuthorsDTO)
        {
            List<AuthorDTO> result = new List<AuthorDTO>();

            if (book.BookAuthor == null) { return result; }

            foreach (BookAuthor bookAuthor in book.BookAuthor)
            {
                result.Add(new AuthorDTO
                {
                    Id = bookAuthor.AuthorId,
                    Name = bookAuthor.Author.Name
                });
            }

            return result;
        }

        private List<BookAuthor> MapBookAuthor (BookCreationDTO bookCreationDTO, Book book)
        {
            List<BookAuthor> result = new List<BookAuthor>();

            if (bookCreationDTO.AuthorIds == null || bookCreationDTO.AuthorIds.Count == 0) { return result; }

            foreach (int authorId in bookCreationDTO.AuthorIds)
            {
                result.Add(new BookAuthor { AuthorId = authorId});
            }
            
            return result;
        }
    }
}
