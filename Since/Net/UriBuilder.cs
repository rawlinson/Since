using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Since.Net
{
    /// <summary>
    /// 
    /// </summary>
    public class UriBuilder
    {
        private string _path;
        private string _query = string.Empty;

        public UriBuilder(string path)
        {
            this._path = path;
        }

        public void AddPath(string path)
        {
            if (!_path.EndsWith("/") && !path.StartsWith("/"))
                _path += '/';
            _path += path;
        }

        public void AddQueryParam(string name, string value)
        {
            _query += string.IsNullOrEmpty(_query) ? '?' : '&';
            _query += name + '=' + value;
        }

        public override string ToString()
        {
            return _path + _query;
        }

        public Uri ToUri()
        {
            return new Uri(this.ToString());
        }
    }


}
