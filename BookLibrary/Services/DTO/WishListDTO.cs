using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTO
{
    public class WishListDTO: IEquatable<WishListDTO>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public string BookId { get; set; }

        public bool Equals(WishListDTO other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            if (Object.ReferenceEquals(this, other)) return true;

            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            int hashAuthorName = Name == null ? 0 : Name.GetHashCode();
            int hashAuthorId = Id.GetHashCode();
            return hashAuthorName ^ hashAuthorId;
        }
    }
}
