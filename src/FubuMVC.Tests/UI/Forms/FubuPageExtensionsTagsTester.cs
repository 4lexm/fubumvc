using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Routing;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Configuration;
using FubuMVC.Core.UI.Tags;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;

namespace FubuMVC.Tests.UI.Forms
{




    public class StubChainAuthorizor : IChainAuthorizor
    {
        public AuthorizationRight Authorize(BehaviorChain chain, object model)
        {
            return AuthorizationRight.Allow;
        }

        public bool IsAuthorized(object model)
        {
            return true;
        }

        public bool IsAuthorized(object model, string category)
        {
            return true;
        }

        public bool IsAuthorized<TController>(Expression<Action<TController>> expression)
        {
            return true;
        }

        public bool IsAuthorizedForNew<T>()
        {
            return true;
        }

        public bool IsAuthorizedForNew(Type entityType)
        {
            return true;
        }

        public bool IsAuthorizedForPropertyUpdate(object model)
        {
            return true;
        }

        public bool IsAuthorizedForPropertyUpdate(Type type)
        {
            return true;
        }

        public bool IsAuthorized(Type handlerType, MethodInfo method)
        {
            return true;
        }
    }

    public abstract class when_calling_text_box_for
    {
    	protected IFubuPage<ViewModel> _page;
		protected IElementNamingConvention _convention;
		protected Expression<Func<ViewModel, object>> _expression;

        public void BaseSetUp()
        {
            _page = MockRepository.GenerateMock<IFubuPage<ViewModel>>();
            _convention = MockRepository.GenerateStub<IElementNamingConvention>();
            _expression = (x => x.Property);
            Accessor accessor = _expression.ToAccessor();
            _convention.Stub(c => c.GetName(Arg<Type>.Is.Equal(typeof(ViewModel)), Arg<Accessor>.Is.Equal(accessor))).Return("name");
            _page.Expect(p => p.Get<IElementNamingConvention>()).Return(_convention);
        }

        public class ViewModel { public string Property { get; set; } }
	}

	[TestFixture]
	public class when_calling_text_box_for_and_model_has_non_null_property : when_calling_text_box_for
	{
		[SetUp]
		public void SetUp()
		{
			BaseSetUp();
			_page.Expect(p => p.Model).Return(new ViewModel { Property = "some value" });
		}

		[Test]
		public void should_return_text_box_tag_with_value()
		{
			_page.TextBoxFor(_expression).ToString().ShouldEqual("<input type=\"text\" name=\"name\" value=\"some value\" />");
			_page.VerifyAllExpectations();
		}
	}

	[TestFixture]
	public class when_calling_text_box_for_and_model_has_null_property : when_calling_text_box_for
	{
		[SetUp]
		public void SetUp()
		{
			BaseSetUp();
			_page.Expect(p => p.Model).Return(new ViewModel());
		}

		[Test]
		public void should_return_text_box_tag_with_blank_value()
		{
			_page.TextBoxFor(_expression).ToString().ShouldEqual("<input type=\"text\" name=\"name\" />");
			_page.VerifyAllExpectations();
		}
	}

    [TestFixture]
    public class when_calling_input_tag_generating_methods
    {
        private IFubuPage<ViewModel> _page;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry(x => x.Import<TestHtmlConventions>());
            var container = new Container(x => x.For<IFubuRequest>().Singleton());


            FubuApplication.For(registry).StructureMap(container).Bootstrap();
            
            var generator = container.GetInstance<TagGenerator<ViewModel>>();
            
            _page = MockRepository.GenerateMock<IFubuPage<ViewModel>>();
            _page.Expect(p => p.Model).Return(new ViewModel());
            _page.Expect(p => p.Get<ITagGenerator<ViewModel>>()).Return(generator);
        }
        
        [Test]
        public void return_html_tag_on_input_for()
        {
            _page.InputFor(x => x.Name).ToString().ShouldEqual("<input type=\"text\" name=\"Name\" />");
        }

        [Test]
        public void return_html_tag_on_label_for()
        {
            _page.LabelFor(x => x.Name).ToString().ShouldEqual("<span class=\"label\">Name</span>");
        }

        [Test]
        public void return_html_tag_on_display_for()
        {
            _page.DisplayFor(x => x.Name).ToString().ShouldEqual("<span></span>");
        }

        public class ViewModel { public string Name { get; set; } }
    }

    [TestFixture]
    public class when_calling_tag_generating_method_for_arbitrary_type
    {
        private IFubuPage _page;
        private ArbitraryModel _modelFromFubuRequest;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry(x =>
            {
                
                x.Import<TestHtmlConventions>();
            });
            var container = new Container(x => x.For<IFubuRequest>().Singleton());

            FubuApplication.For(registry).StructureMap(container).Bootstrap();
            
            var generator = container.GetInstance<TagGenerator<ArbitraryModel>>();
            _page = MockRepository.GenerateMock<IFubuPage>();
            _page.Stub(p => p.Get<ITagGenerator<ArbitraryModel>>()).Return(generator);
            var fubuRequest = MockRepository.GenerateMock<IFubuRequest>();
            _modelFromFubuRequest = new ArbitraryModel{City="Austin"};
            fubuRequest.Stub(x => x.Get<ArbitraryModel>()).Return(_modelFromFubuRequest);
            _page.Stub(p => p.Get<IFubuRequest>()).Return(fubuRequest);
        }

        [Test]
        public void should_populate_the_input_using_a_model_from_fuburequest()
        {
            var tag = _page.InputFor<ArbitraryModel>(x => x.City);
            tag.TagName().ShouldEqual("input");
            tag.Attr("value").ShouldEqual(_modelFromFubuRequest.City);
        }

        [Test]
        public void should_display_the_name_of_the_property_on_the_type()
        {
            var tag = _page.LabelFor<ArbitraryModel>(x => x.City);
            tag.TagName().ShouldEqual("span");
            tag.Text().ShouldEqual("City");
        }

        [Test]
        public void should_display_the_value_of_the_property_from_fuburequest()
        {
            var tag = _page.DisplayFor<ArbitraryModel>(x => x.City);
            tag.TagName().ShouldEqual("span");
            tag.Text().ShouldEqual(_modelFromFubuRequest.City);
        }

        public class ArbitraryModel { public string City { get; set; } }
    }

    [TestFixture]
    public class when_calling_tag_generating_method_for_given_model
    {
        private IFubuPage _page;
        private ArbitraryModel _givenModel;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry(x => x.Import<TestHtmlConventions>());
            var container = new Container(x => x.For<IFubuRequest>().Singleton());


            FubuApplication.For(registry).StructureMap(container).Bootstrap();

            var generator = container.GetInstance<TagGenerator<ArbitraryModel>>();
            _page = MockRepository.GenerateMock<IFubuPage>();
            _page.Stub(p => p.Get<ITagGenerator<ArbitraryModel>>()).Return(generator);
            _givenModel = new ArbitraryModel { City = "Austin" };
        }

        [Test]
        public void should_populate_the_input_using_the_given_model()
        {
            var tag = _page.InputFor(_givenModel, x => x.City);
            tag.TagName().ShouldEqual("input");
            tag.Attr("value").ShouldEqual(_givenModel.City);
        }

        [Test]
        public void should_display_the_name_of_the_property_on_the_type()
        {
            var tag = _page.LabelFor(_givenModel, x => x.City);
            tag.TagName().ShouldEqual("span");
            tag.Text().ShouldEqual("City");
        }

        [Test]
        public void should_display_the_value_of_the_property_from_the_given_model()
        {
            var tag = _page.DisplayFor(_givenModel, x => x.City);
            tag.TagName().ShouldEqual("span");
            tag.Text().ShouldEqual(_givenModel.City);
        }

        public class ArbitraryModel { public string City { get; set; } }
    }
}