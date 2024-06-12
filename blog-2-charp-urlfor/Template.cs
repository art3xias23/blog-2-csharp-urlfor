using Scriban;
using Scriban.Runtime;

namespace blog_2_charp_urlfor
{
    public class TemplateParser : ITemplateParser
    {
        public string Render(object model, string filePath)
        {
            Template template;
            if (File.Exists(filePath))
            {

                var text = File.ReadAllText(filePath);
                template = Template.Parse(text);
            }
            else
            {
                template = Template.Parse(filePath);
            }

            var context = new TemplateContext
            {
                MemberRenamer = member => member.Name
            };
            if (model != null)
            {
                var scriptObject = new ScriptObject();
                scriptObject.Import(model);
                context.PushGlobal(scriptObject);
            }

            return template.Render(context);
        }
    }
}
