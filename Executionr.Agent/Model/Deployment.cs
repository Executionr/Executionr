using System;
using System.ComponentModel.DataAnnotations;

namespace Executionr.Agent.Model
{
	public class Deployment
	{
        [Required]
        public int Id { get; set; }

        [Required]
        public string Version { get; set; }

        [Required]
        public Uri Url { get; set; }

        [Required]
        public string Hash { get; set; }
	}
}

