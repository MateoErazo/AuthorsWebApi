﻿using AuthorsWebApi.Validations;
using System.ComponentModel.DataAnnotations;

namespace AuthorsWebApi.DTOs
{
    public class AuthorDTO: Resource
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }
}
