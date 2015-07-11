using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Web;
using System.Web.Caching;
using Nancy;
using Nancy.ModelBinding;
using Newtonsoft.Json;

namespace RandomSAQs.Modules
{
    public class QuestionsModule : NancyModule
    {
        private readonly IEnumerable<ShortAnswerQuestion> _questions;

        public QuestionsModule()
        {
            _questions = GetQuestions();

            Get["/questions/{count:int}"] = _ =>
            {
                int count = _.count;
                var response = GetRandomQuestions(count);
                return new ShortAnswerQuestionCollection(response);
            };
            Get["/questions"] = _ => Response.AsRedirect("/questions/8");

            Get["/question/{id:int}"] = _ =>
            {
                var id = _.id;
                var question = _questions.SingleOrDefault(q => q.Id == id);
                return question;
            };

            Post["/questions/{id:int}"] = _ =>
            {
                var question = this.Bind<ShortAnswerQuestion>();
                int id = _.id;
                if (_questions.Any(q => q.Id == id))
                {
                    _questions.Single(q => q.Id == id).Include = question.Include;
                    _questions.Single(q => q.Id == id).QuestionText = question.QuestionText;
                    return Response.AsJson(question, HttpStatusCode.Accepted);
                }
                var response = FormatterExtensions.AsText(Response, "Item with Id " + id + " not found");
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            };

            Post["/question/report/{id:int}"] = _ =>
            {
                var id = _.id;
                var question = _questions.SingleOrDefault(q => q.Id == id);
                if (question != null)
                {
                    question.Include = false;
                    return Response.AsJson(question, HttpStatusCode.Accepted);
                }
                var response = FormatterExtensions.AsText(Response, "Item with Id " + id + " not found");
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            };
        }

        private IEnumerable<ShortAnswerQuestion> GetRandomQuestions(int count)
        {
            var questions = new List<ShortAnswerQuestion>();
            var rnd = new Random();
            var safeguard = 0;
            while (questions.Count < count && safeguard < 1000)
            {
                var index = rnd.Next(0, _questions.Count() - 1);
                var question = _questions.ElementAt(index);
                if (question.Include)
                {
                    questions.Add(new ShortAnswerQuestion
                    {
                        Id = question.Id,
                        QuestionText = question.QuestionText,
                        Include = true,
                    });
                }
                safeguard++;
            }

            return questions;
        }

        private IEnumerable<ShortAnswerQuestion> GetQuestions()
        {
            var cached = HttpContext.Current.Cache.Get("questions");
            if (cached != null)
                return cached as IEnumerable<ShortAnswerQuestion>;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RandomSAQs.formatted.json"))
            {
                using (var reader = new StreamReader(stream))
                {
                    var result = reader.ReadToEnd();
                    var questions = JsonConvert.DeserializeObject<List<ShortAnswerQuestion>>(result);
                    HttpContext.Current.Cache.Add("questions", questions, null, Cache.NoAbsoluteExpiration,
                        TimeSpan.FromHours(24), CacheItemPriority.Normal, null);
                    return questions;
                }
            }
        }

    }
}