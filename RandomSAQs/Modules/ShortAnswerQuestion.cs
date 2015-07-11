using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace RandomSAQs.Modules
{
    public class ShortAnswerQuestion
    {
        public int Id { get; set; }
        [JsonProperty("question")]
        public string QuestionText { get; set; }

        public bool Include { get; set; }
    }

    public class ShortAnswerQuestionCollection : IEnumerable<ShortAnswerQuestion>
    {
        private readonly IEnumerable<ShortAnswerQuestion> _questions;

        public ShortAnswerQuestionCollection(IEnumerable<ShortAnswerQuestion> questions)
        {
            _questions = questions;
        }

        public IEnumerator<ShortAnswerQuestion> GetEnumerator()
        {
            return _questions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}