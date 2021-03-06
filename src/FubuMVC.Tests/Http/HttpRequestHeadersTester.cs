using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Http;
using NUnit.Framework;
using FubuTestingSupport;
using System.Collections.Generic;

namespace FubuMVC.Tests.Http
{
    [TestFixture]
    public class HttpRequestHeadersTester
    {
        [Test]
        public void smoke_test_of_the_name_lookup()
        {
            HttpRequestHeaders.HeaderNameFor(HttpRequestHeader.Warning)
                .ShouldEqual("Warning");

            HttpRequestHeaders.HeaderNameFor(HttpRequestHeader.IfNoneMatch)
                .ShouldEqual("If-None-Match");
        }

        [Test]
        public void naming_strategy_positive()
        {
            var propertyInfo = ReflectionHelper.GetProperty<ETagDto>(x => x.IfModifiedSince);
            HttpRequestHeaders.HeaderDictionaryNameForProperty(propertyInfo.Name)
                .ShouldEqual("If-Modified-Since");
        }

        [Test]
        public void naming_strategy_negative_is_a_passthru()
        {
            // Not sure what a null will do to us in the model binding,
            // so I'm having it return the property name
            HttpRequestHeaders.HeaderDictionaryNameForProperty("something")
                .ShouldEqual("something");
        }


        public class ETagDto
        {
            public string IfMatch { get; set; }
            public DateTime IfModifiedSince { get; set; }
            public string IfNoneMatch { get; set; }
        }
    }
}