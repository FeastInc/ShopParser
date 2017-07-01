using HtmlAgilityPack;

namespace DataParser.HelperClasses
{
    public static class HtmlAgilityPackExtension
    {
        public static HtmlNodeCollection _SelectNodes(this HtmlNode node, string xPath)
        {
            return node.SelectNodes(xPath) ?? new HtmlNodeCollection(node);
        }
    }
}
