using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexGym.Domain.Common
{
    public class NotFoundException : Exception
    {
        // These are constructors - special methods for creating the exception.

        // Constructor 1: Basic, parameterless. Not very useful for us.
        public NotFoundException() : base()
        {
        }

        // Constructor 2: Lets us create the exception with a custom message.
        public NotFoundException(string message) : base(message)
        {
        }

        // Constructor 3: For advanced scenarios where one exception causes another.
        public NotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        // Constructor 4: THE ONE WE WILL USE ALL THE TIME.
        // It takes the name of the entity (e.g., "Member") and its key (e.g., the ID 9999).
        // It then builds a helpful message like "Entity 'Member' (9999) was not found."
        public NotFoundException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.")
        {
        }
    }
}
