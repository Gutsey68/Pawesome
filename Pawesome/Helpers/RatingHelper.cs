using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace Pawesome.Helpers
{
    public static class RatingHelper
    {
        public static IHtmlContent DisplayStars(this IHtmlHelper helper, float? rating, int maxRating = 5)
        {
            if (rating == null)
            {
                return new HtmlString("<span class=\"no-rating\">Pas encore noté</span>");
            }

            var stars = new StringBuilder();
            stars.Append("<div class=\"rating-stars\">");
            
            string starSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"24\" height=\"24\" viewBox=\"0 0 24 24\" " +
                             "fill=\"currentColor\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" " +
                             "stroke-linejoin=\"round\" class=\"star-icon\">" +
                             "<path d=\"M11.525 2.295a.53.53 0 0 1 .95 0l2.31 4.679a2.123 2.123 0 0 0 1.595 1.16l5.166.756a.53.53 0 0 1 " +
                             ".294.904l-3.736 3.638a2.123 2.123 0 0 0-.611 1.878l.882 5.14a.53.53 0 0 1-.771.56l-4.618-2.428a2.122 " +
                             "2.122 0 0 0-1.973 0L6.396 21.01a.53.53 0 0 1-.77-.56l.881-5.139a2.122 2.122 0 0 0-.611-1.879L2.16 9.795a" +
                             ".53.53 0 0 1 .294-.906l5.165-.755a2.122 2.122 0 0 0 1.597-1.16z\"/></svg>";
            
            int fullStars = (int)Math.Floor(rating.Value);
            bool hasHalfStar = (rating.Value - fullStars) >= 0.5;
            
            for (int i = 0; i < fullStars; i++)
            {
                stars.Append($"<span class=\"star filled\">{starSvg}</span>");
            }
            
            if (hasHalfStar && fullStars < maxRating)
            {
                stars.Append($"<span class=\"star half-filled\">{starSvg}</span>");
                fullStars++;
            }
            
            for (int i = fullStars + (hasHalfStar ? 0 : 0); i < maxRating; i++)
            {
                stars.Append($"<span class=\"star empty\">{starSvg}</span>");
            }
            
            stars.Append($"<span class=\"rating-value\">{rating:F1}</span>");
            stars.Append("</div>");
            
            return new HtmlString(stars.ToString());
        }
    }
}