using System;
using System.Xml;

namespace Tharga.Reporter.Engine.Entity
{
    public class Template
    {
        public readonly SectionList SectionList = new SectionList();

        private Template()
        {
        }

        public Template(Section section)
        {
            if (section == null) throw new ArgumentNullException("section");

            SectionList.Add(section);
        }

        public static Template Load(XmlDocument document)
        {
            var template = new Template();

            if (document.ChildNodes.Count == 0) throw new InvalidOperationException("There are no child nodes on the root document.");
            if (document.ChildNodes.Count > 1) throw new InvalidOperationException("There are more than one child node on the root document.");

            var xmlTemplate = document.FirstChild;

            if (xmlTemplate.Name != "Template") throw new InvalidOperationException(string.Format("Template level cannot be parsed as element of type {0}.", xmlTemplate.Name));
            if (xmlTemplate.ChildNodes.Count == 0) throw new InvalidOperationException("There have to be at least one section in the template.");
            foreach (XmlElement xmlSection in xmlTemplate.ChildNodes)
            {
                if (xmlSection.Name != "Section") throw new InvalidOperationException(string.Format("Section level cannot parsed as element of type {0}.", xmlSection.Name));
                var sec = Section.Load(xmlSection);
                template.SectionList.Add(sec);
            }

            return template;
        }

        public XmlDocument ToXml()
        {
            var document = new XmlDocument();

            var template = document.CreateElement("Template");
            document.AppendChild(template);

            if (SectionList.Count == 0) throw new InvalidOperationException("There have to be at least one section in the template.");

            foreach (var section in SectionList)
            {
                var xmeSection = section.ToXme();
                var imported = document.ImportNode(xmeSection, true);
                template.AppendChild(imported);
            }

            return document;
        }
    }
}