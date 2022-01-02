using System.Text;

namespace SteamUtils.Models
{
    public record SearchMarketRequest(
        string Query = "",
        string ItemClass = "tag_item_class_2",
        string SortColumn = "price",
        string SortDirection = "asc",
        string AppId = "753",
        string Game = "any",
        int Start = 1,
        int Count = 100
    )
    {
        public string GetQueryParams()
        {
            var sb = new StringBuilder("/market/search/render/");

            sb.AppendFormat("?q={0}", Query);
            sb.AppendFormat("&category_753_Game%5B%5D={0}", Game);
            sb.AppendFormat("&category_753_item_class%5B%5D={0}", ItemClass);
            sb.AppendFormat("&appid={0}", AppId);

            sb.AppendFormat("&start={0}", Start.ToString());
            sb.AppendFormat("&count={0}", Count.ToString());
            sb.AppendFormat("&sort_column={0}", SortColumn);
            sb.AppendFormat("&sort_dir={0}", SortDirection);

            sb.Append("&norender=1");

            return sb.ToString();
        }
    }
}