using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MovieCatalog.Tests.DTOs
{
    public class ApiResponseDto
    {
        [JsonPropertyName("msg")]
        public required string Msg { get; set; }

        [JsonPropertyName("movie")]
        public MovieDto Movie { get; set; } = new MovieDto();
    }
}
