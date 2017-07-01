using HtmlAgilityPack;

namespace DataParser.HelperClasses
{
    public delegate TResult Search<out TResult>(HtmlNode node, ArgumentObject args);
}
