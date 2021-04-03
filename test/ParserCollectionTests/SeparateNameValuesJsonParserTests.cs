using HubCloud.ParserCollection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParserCollectionTests
{
    [TestFixture]
    public class SeparateNameValuesJsonParserTests
    {
        [Test]
        public void Parse_NoDoubleNamesNoMapping_ReturnData()
        {
            var settings = new SeparateNameValuesJsonParserSettings()
            {
                NamesPath = "info.fields",
                ValuesPath = "info.items"
            };

            var parser = new SeparateNameValuesJsonParser(settings);
            var result = parser.Parse(TestData.ResourceManager.GetString("SeparateNameValuesExample"));

            Assert.AreEqual(2, result.Count);

            var item1 = result[0];

            Assert.AreEqual(6, item1.Count);
            Assert.AreEqual("1", item1["id"]);
        }

        [Test]
        public void Parse_IsDoubleIgnoreDoublesNamesNoMapping_ReturnData()
        {
            var settings = new SeparateNameValuesJsonParserSettings()
            {
                NamesPath = "info.fields",
                ValuesPath = "info.items"
            };

            var parser = new SeparateNameValuesJsonParser(settings);
            var result = parser.Parse(TestData.ResourceManager.GetString("SeparateNameValuesWithDoublesExample"));

            Assert.AreEqual(2, result.Count);

            var item1 = result[0];

            Assert.AreEqual(7, item1.Count);
            Assert.AreEqual("s-11", item1["Search key"]);
        }

        [Test]
        public void Parse_IsDoubleTakeDoublesNamesNoMapping_ReturnData()
        {
            var settings = new SeparateNameValuesJsonParserSettings()
            {
                NamesPath = "info.fields",
                ValuesPath = "info.items",
                IgnoreDoubleNames = false
            };

            var parser = new SeparateNameValuesJsonParser(settings);
            var result = parser.Parse(TestData.ResourceManager.GetString("SeparateNameValuesWithDoublesExample"));

            Assert.AreEqual(2, result.Count);

            var item1 = result[0];

            Assert.AreEqual(8, item1.Count);
            Assert.AreEqual("s-11", item1["Search key"]);
            Assert.AreEqual("s-12", item1["Search key2"]);

        }

        [Test]
        public void Parse_NoDoubleNamesIsMapping_ReturnData()
        {
            var settings = new SeparateNameValuesJsonParserSettings()
            {
                NamesPath = "info.fields",
                ValuesPath = "info.items"
            };
            settings.NamesMapping.Add("id", "id");
            settings.NamesMapping.Add("Email", "mail");
            settings.NamesMapping.Add("Phone", "tel");

            var parser = new SeparateNameValuesJsonParser(settings);
            var result = parser.Parse(TestData.ResourceManager.GetString("SeparateNameValuesExample"));

            Assert.AreEqual(2, result.Count);

            var item1 = result[0];

            Assert.AreEqual(3, item1.Count);
            Assert.AreEqual("+7123456789", item1["tel"]);
        }

        [Test]
        public void Parse_IsDoubleTakeDoublesNamesIsMapping_ReturnData()
        {
            var settings = new SeparateNameValuesJsonParserSettings()
            {
                NamesPath = "info.fields",
                ValuesPath = "info.items",
                IgnoreDoubleNames = false
            };

            settings.NamesMapping.Add("id", "id");
            settings.NamesMapping.Add("Email", "mail");
            settings.NamesMapping.Add("Phone", "tel");
            settings.NamesMapping.Add("Search key", "search_1");
            settings.NamesMapping.Add("Search key2", "search_2");

            var parser = new SeparateNameValuesJsonParser(settings);
            var result = parser.Parse(TestData.ResourceManager.GetString("SeparateNameValuesWithDoublesExample"));

            Assert.AreEqual(2, result.Count);

            var item1 = result[0];

            Assert.AreEqual(5, item1.Count);
            Assert.AreEqual("s-11", item1["search_1"]);
            Assert.AreEqual("s-12", item1["search_2"]);

        }

        [Test]
        public void Parse_IsArrays_ReturnData()
        {
            var settings = new SeparateNameValuesJsonParserSettings()
            {
                NamesPath = "info.fields",
                ValuesPath = "info.items",
                IgnoreDoubleNames = true
            };
            settings.NamesMapping.Add("id", "id");
            settings.NamesMapping.Add("Email", "mail");
            settings.NamesMapping.Add("Tags", "tags");


            var parser = new SeparateNameValuesJsonParser(settings);
            var result = parser.Parse(TestData.ResourceManager.GetString("SeparateNameValuesWithInnerArrays"));

            Assert.AreEqual(2, result.Count);

            var item2 = result[1];

            Assert.AreEqual(3, item2.Count);
            Assert.AreEqual("tag_1, tag_2", item2["tags"]);

        }
    }
}
