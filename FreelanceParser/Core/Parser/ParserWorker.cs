using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FreelanceParser.Core
{
    public class ParserWorker<T> where T : class
    {
        private IParser<T> _parser;
        private IParserSettings _parserSettings;

        private HtmlLoader loader;

        private bool isActive;

        #region Properties

        public IParser<T> Parser
        {
            get => _parser;

            set
            {
                _parser = value;
            }
        }

        public IParserSettings Settings
        {
            get => _parserSettings;

            set
            {
                _parserSettings = value;
                loader = new HtmlLoader(value);
            }
        }

        public bool IsActive
        {
            get => isActive;
        }

        #endregion

        public event Action<object, T> OnNewData;
        public event Action<IEnumerable<T>> OnCompleted;

        public ParserWorker(IParser<T> parser)
        {
            _parser = parser;
        }

        public ParserWorker(IParser<T> parser, IParserSettings parserSettings) : this(parser)
        {
            _parserSettings = parserSettings;
        }

        public void Start()
        {
            isActive = true;
            WorkerAsync();
        }

        public void Abort()
        {
            isActive = false;
        }

        private async void WorkerAsync()
        {
            var allItems = new List<T>();

            int pageId = 1;

            if (!isActive)
            {
                OnCompleted?.Invoke(new T[0]);

                return;
            }

            var source = await loader.GetSourceByPageId(pageId);
            var domParser = new HtmlParser();

            var document = await domParser.ParseDocumentAsync(source);

            var result = _parser.Parse(document);

            allItems.AddRange(result);

            foreach (var item in result)
            {
                OnNewData?.Invoke(this, item);
            }

            OnCompleted?.Invoke(allItems);
            isActive = false;
        }
    }
}