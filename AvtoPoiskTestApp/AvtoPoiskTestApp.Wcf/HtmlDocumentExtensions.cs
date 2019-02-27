using mshtml;

namespace AvtoPoiskTestApp.Wcf
{
    public static class HtmlDocumentExtensions
    {
        public static void SetValueToElementById(this HTMLDocument doc, string id, string value)
        {
            var el = doc?.getElementById(id);
            el?.setAttribute("value", value);
        }

        public static void ClickElementWithId(this HTMLDocument doc, string id)
        {
            var el = doc?.getElementById(id);
            el?.click();
        }

        public static void RemoveElementWithId(this HTMLDocument doc, string id)
        {
            var el = doc?.getElementById(id);

            if (!(el is IHTMLDOMNode childNode))
            {
                return;
            }

            var parentNode = childNode.parentNode;
            parentNode.removeChild(childNode);
        }
    }
}
