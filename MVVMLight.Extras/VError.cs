using System.Collections;
using System.Collections.Generic;

namespace MVVMLight.Extras
{
    class VError:IEnumerable
    {
        public VError()
        {
            Errors = new List<string>();
        }
        public List<string> Errors { get; set; }
        public IEnumerator GetEnumerator()
        {
            return Errors.GetEnumerator();
        }
    }
}