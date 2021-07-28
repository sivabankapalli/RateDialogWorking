using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAssistant1.Models
{
	public class RateAnswerState
	{
		public string Question { get; set; }

		public RateAnswerState()
		{

		}

		public RateAnswerState(string question)
		{
			Question = question;
		}
	}
}