using System;
using System.Dynamic;
using Xunit;

namespace TemplateR.Core.Test
{
    public class CustomDataConverterTest
    {
        [Fact]
        public void TestDateStringConverter_V1()
        {
            var templateString = "<html>{{Data1}}</html>";
            var date = DateTime.Now;
            var templateBuilder = new TemplateBuilder(new Config.Configuration().RegisterConverter(new DateTimeDataConverter()));
            var template = templateBuilder.FromString(templateString);
            var result = template.FillTemplate(new { Data1 = date });

            Assert.Equal($"<html>{date.Year}</html>", result);
        }

        [Fact]
        public void TestDateStringConverter_V2()
        {
            var templateString = "<html>{{ Data1}}</html>";
            var date = DateTime.Now;
            var templateBuilder = new TemplateBuilder(new Config.Configuration().RegisterConverter(new DateTimeDataConverter()));
            var template = templateBuilder.FromString(templateString);
            var result = template.FillTemplate(new { Data1 = date });

            Assert.Equal($"<html>{date.Year}</html>", result);
        }

        [Fact]
        public void TestDateStringConverter_V3()
        {
            var templateString = "<html>{{Data1 }}</html>";
            var date = DateTime.Now;
            var templateBuilder = new TemplateBuilder(new Config.Configuration().RegisterConverter(new DateTimeDataConverter()));
            var template = templateBuilder.FromString(templateString);
            var result = template.FillTemplate(new { Data1 = date });

            Assert.Equal($"<html>{date.Year}</html>", result);
        }

        [Fact]
        public void TestDateStringConverter_V4()
        {
            var templateString = "<html>{{       Data1         }}</html>";
            var date = DateTime.Now;
            var templateBuilder = new TemplateBuilder(new Config.Configuration().RegisterConverter(new DateTimeDataConverter()));
            var template = templateBuilder.FromString(templateString);
            var result = template.FillTemplate(new { Data1 = date });

            Assert.Equal($"<html>{date.Year}</html>", result);
        }

        [Fact]
        public void TestDefaultDateStringConverter_V1()
        {
            var templateString = "<html>{{Data1:yyyy.MM.dd}}</html>";
            var date = DateTime.Now;
            var templateBuilder = new TemplateBuilder();
            var template = templateBuilder.FromString(templateString);
            var result = template.FillTemplate(new { Data1 = date });

            Assert.Equal($"<html>{date:yyyy.MM.dd}</html>", result);
        }

        [Fact]
        public void TestDefaultDateStringConverter_V2()
        {
            var templateString = "<html>{{      Data1:yyyy.MM.dd     }}</html>";
            var date = DateTime.Now;
            var templateBuilder = new TemplateBuilder();
            var template = templateBuilder.FromString(templateString);
            var result = template.FillTemplate(new { Data1 = date });

            Assert.Equal($"<html>{date:yyyy.MM.dd}</html>", result);
        }
    }

    public class DataNavigationTest
    {
        [Fact]
        public void TestDeepObjects_V1()
        {
            var templateString = "<html>{{X1.X2.X3:yyyy.MM.dd}}</html>";
            var date = DateTime.Now;
            var templateBuilder = new TemplateBuilder();
            var template = templateBuilder.FromString(templateString);
            var result = template.FillTemplate(new { X1 = new { X2 = new { X3 = date } } });

            Assert.Equal($"<html>{date:yyyy.MM.dd}</html>", result);
        }

        [Fact]
        public void TestDeepObjects_V2()
        {
            var templateString = "<html>{{    X1.X2:yyyy.MM.dd     }}</html>";
            var date = DateTime.Now;
            var templateBuilder = new TemplateBuilder();
            var template = templateBuilder.FromString(templateString);
            var result = template.FillTemplate(new { X1 = new { X2 = date } });

            Assert.Equal($"<html>{date:yyyy.MM.dd}</html>", result);
        }
    }
}
