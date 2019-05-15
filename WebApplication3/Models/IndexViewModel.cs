using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication3.Models
{
    public class IndexViewModel
    {
        public IEnumerable<string> ProjectsFromFileSystem { get; set; }

        public IEnumerable<Tuple<int, string>> ProjectsFromRepository { get; set; }
    }
}