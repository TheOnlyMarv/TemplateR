using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace TemplateR.Core.Test
{
    public class EnumerationTest
    {
        [Fact]
        public void TestEnumeration_V1()
        {
            var templateString = "<html>{{Data[0]}}</html>";
            var templateBuilder = new TemplateBuilder();
            var template = templateBuilder.FromString(templateString);
            var result = template.FillTemplate(new { Data = new int[] { 1, 2 } });

            Assert.Equal($"<html>1</html>", result);
        }

        [Fact]
        public void TestEnumeration_V2()
        {
            var templateString = "<html>{{Data[1]}}</html>";
            var templateBuilder = new TemplateBuilder();
            var template = templateBuilder.FromString(templateString);
            var result = template.FillTemplate(new { Data = new int[] { 1, 2 } });

            Assert.Equal($"<html>2</html>", result);
        }

        [Fact]
        public void TestEnumeration_V3()
        {
            var templateString = "<html>{{Data[0]:yyyy.MM.dd}}</html>";
            var date = DateTime.Now;
            var templateBuilder = new TemplateBuilder();
            var template = templateBuilder.FromString(templateString);
            var result = template.FillTemplate(new { Data = new DateTime[] { date } });

            Assert.Equal($"<html>{date:yyyy.MM.dd}</html>", result);
        }

        [Fact]
        public void TestEnumeration_V4()
        {
            var templateString = "<html>{{Data[1].Data1}}</html>";
            var templateBuilder = new TemplateBuilder();
            var template = templateBuilder.FromString(templateString);
            var result = template.FillTemplate(new { Data = new[] { new { Data0 = 5, Data1 = 7 }, new { Data0 = 3, Data1 = 8 }, new { Data0 = 2, Data1 = 4 } } });

            Assert.Equal($"<html>8</html>", result);
        }

        [Fact]
        public void TestEnumeration_V5()
        {
            var templateString = "<html>{{B[1].A[0]}}</html>";
            var templateBuilder = new TemplateBuilder();
            var template = templateBuilder.FromString(templateString);
            var data = new { A = new { }, B = new object[] { new { }, new { A = new[] { 1, 2, 3 } } }, C = new { } };
            var result = template.FillTemplate(data);

            Assert.Equal($"<html>1</html>", result);
        }
    }
}
