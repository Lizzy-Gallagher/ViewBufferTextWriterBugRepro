using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ViewBufferTextWriterBugRepro.HtmlHelpers
{
	public static class Extensions
	{
		public static HtmlHelpers HtmlHelpers(this IHtmlHelper htmlHelper)
		{
			return new HtmlHelpers(htmlHelper);
		}
	}

	public class HtmlHelpers
	{
		private readonly IHtmlHelper _htmlHelper;

		internal HtmlHelpers(IHtmlHelper htmlHelper)
		{
			this._htmlHelper = htmlHelper;
		}

		public void CreateRepro(IHtmlContent text)
		{
			// In production code, we are doing something more complicated with this
			// ViewContext.Writer and would like to continue using it if possible.
			var writer = this._htmlHelper.ViewContext.Writer;
			
			var headerTextDiv = new TagBuilder("div");
			writer.Write(headerTextDiv.RenderStartTag());
			
			headerTextDiv.InnerHtml.AppendHtml(text);
			headerTextDiv.WriteTo(writer, HtmlEncoder.Default);

			writer.Write(headerTextDiv.RenderEndTag());

			// NOTE: This issue is only present when the text is inside
			// a TagBuilder. The following does not throw the ArgumentOutOfRangeException:
			// writer.Write(text);
		}
	}
}
