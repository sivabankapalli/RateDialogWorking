using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAssistant1.Models
{
	public class AnswerRatingDto
	{
		public string Question { get; private set; }

		public AnswerRatingDto(string question)
		{
			Question = question;
		}
	}
}
