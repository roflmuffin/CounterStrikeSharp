using System.Text;
using System.Text.RegularExpressions;

namespace CounterStrikeSharp.API
{
	public static class StringExtensions
	{
		private const string HTML_TAG_REGEX_PATTERN = "<[^>]+>";
		private static readonly Regex TagRegex = new(HTML_TAG_REGEX_PATTERN, RegexOptions.Compiled);
		
		public static string TruncateHtml(this string msg, int maxLength)
		{
			if (maxLength <= 0)
				return msg;

			if (string.IsNullOrEmpty(msg))
				return string.Empty;

			string textOnly = Regex.Replace(msg, HTML_TAG_REGEX_PATTERN, "");
			if (textOnly.Length <= maxLength)
				return msg;

			Stack<string> tagStack = new Stack<string>();
			StringBuilder result = new System.Text.StringBuilder();
			int visibleLength = 0,
				i = 0;

			while (i < msg.Length && visibleLength < maxLength)
			{
				if (msg[i] == '<')
				{
					Match match = TagRegex.Match(msg, i);
					if (match.Success && match.Index == i)
					{
						string tag = match.Value;
						result.Append(tag);
						i += tag.Length;

						if (!tag.StartsWith("</")) // Opening tag
						{
							string tagName = tag.Split(new[] { ' ', '>' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim('<');
							if (!tag.EndsWith("/>") && !tagName.StartsWith("!"))
								tagStack.Push(tagName);
						}
						else if (tagStack.Count > 0)
						{
							tagStack.Pop();
						}

						continue;
					}
				}
				else
				{
					result.Append(msg[i]);
					visibleLength++;
				}

				i++;
			}

			while (tagStack.Count > 0)
				result.Append($"</{tagStack.Pop()}>");

			return result.ToString();
		}
	}
}
