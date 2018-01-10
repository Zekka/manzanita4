using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manzanita4
{
    class Identity
    {
        private readonly string _name;
        private readonly string _parameterization;
        private readonly Identity[] _dependencies;
        public IEnumerable<Identity> Dependencies => _dependencies;
        
        public Identity(string name, string parameterization, params Identity[] dependencies)
        {
            _name = name;
            _parameterization = parameterization;
            _dependencies = (Identity[]) dependencies.Clone();
        }

        protected bool Equals(Identity other)
        {
            return string.Equals(_name, other._name) && string.Equals(_parameterization, other._parameterization) && _dependencies.SequenceEqual(other._dependencies);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Identity) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _name?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (_parameterization?.GetHashCode() ?? 0);
                foreach (var dep in _dependencies) { hashCode = (hashCode * 397) ^ dep.GetHashCode(); }
                return hashCode;
            }
        }

        public static bool operator ==(Identity left, Identity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Identity left, Identity right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder(_name);
            if (_parameterization != "")
            {
                output.Append($"[{_parameterization}]");
            }
            if (Dependencies.Any())
            {
                output.Append("(");
                output.Append(string.Join(" ", from d in Dependencies select d.ToString()));
                output.Append(")");
            }
            return output.ToString();
        }

        public string ToShortString()
        {
            StringBuilder output = new StringBuilder(_name);
            if (_parameterization != "")
            {
                output.Append($"[{_parameterization}]");
            }
            if (Dependencies.Any())
            {
                output.Append("(");
                output.Append(string.Join(" ", from d in Dependencies select d.ToShortestString()));
                output.Append(")");
            }
            return output.ToString();
        }

        public string ToShortestString()
        {
            StringBuilder output = new StringBuilder(_name);
            if (_parameterization != "")
            {
                output.Append($"[{_parameterization}]");
            }
            if (Dependencies.Any())
            {
                output.Append("(...)");
            }
            return output.ToString();
        }
    }
}
