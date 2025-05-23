namespace Pawesome.Helpers
{
    /// <summary>
    /// Provides helper methods for generating image URLs for users and pets.
    /// </summary>
    public static class ImageHelper
    {
        /// <summary>
        /// Returns the URL for a user's photo.
        /// If the photo path is null or empty, returns the placeholder avatar image URL.
        /// If the photo path starts with "external:", returns the external URL.
        /// If the photo path starts with "http", returns it as is (external).
        /// If the photo path starts with "/images/users/", returns it as is.
        /// Otherwise, returns the local user image URL.
        /// </summary>
        /// <param name="photoPath">The path or external URL of the user's photo.</param>
        /// <returns>The resolved photo URL as a string.</returns>
        public static string GetPhotoUrl(string? photoPath)
        {
            if (string.IsNullOrEmpty(photoPath))
                return "/images/placeholder-avatar.png";

            if (photoPath.StartsWith("external:"))
                return photoPath.Substring(9);

            if (photoPath.StartsWith("http", System.StringComparison.OrdinalIgnoreCase))
                return photoPath;
                
            if (photoPath.StartsWith("/images/users/", System.StringComparison.OrdinalIgnoreCase))
                return photoPath;

            return $"/images/users/{photoPath}";
        }

        /// <summary>
        /// Returns the URL for a pet's photo.
        /// If the photo path is null or empty, returns the placeholder pet image URL.
        /// If the photo path starts with "external:", returns the external URL.
        /// If the photo path starts with "http", returns it as is (external).
        /// If the photo path starts with "/images/pets/", returns it as is.
        /// Otherwise, returns the local pet image URL.
        /// </summary>
        /// <param name="photoPath">The path or external URL of the pet's photo.</param>
        /// <returns>The resolved pet photo URL as a string.</returns>
        public static string GetPetPhotoUrl(string? photoPath)
        {
            if (string.IsNullOrEmpty(photoPath))
                return "/images/placeholder-pet.png";

            if (photoPath.StartsWith("external:"))
                return photoPath.Substring(9);
                
            if (photoPath.StartsWith("http", System.StringComparison.OrdinalIgnoreCase))
                return photoPath;
                
            if (photoPath.StartsWith("/images/pets/", System.StringComparison.OrdinalIgnoreCase))
                return photoPath;

            return $"/images/pets/{photoPath}";
        }
    }
}