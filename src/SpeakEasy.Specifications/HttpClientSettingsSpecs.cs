using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;
using SpeakEasy.ArrayFormatters;
using SpeakEasy.Authenticators;
using SpeakEasy.Middleware;
using SpeakEasy.Serializers;

namespace SpeakEasy.Specifications
{
    [Subject(typeof(HttpClientSettings))]
    class HttpClientSettingsSpecs
    {
        static HttpClientSettings settings;

        Establish context = () =>
            settings = new HttpClientSettings();

        class in_general
        {
            It should_have_null_authenticator = () =>
                settings.Authenticator.ShouldBeOfExactType<NullAuthenticator>();

            It should_have_default_naming_convention = () =>
                settings.NamingConvention.ShouldBeOfExactType<DefaultNamingConvention>();

            It should_have_default_user_agent = () =>
                settings.Middleware.HasMiddleware<UserAgentMiddleware>().ShouldBeTrue();

            It should_have_default_array_formatter = () =>
                settings.ArrayFormatter.ShouldBeOfExactType<MultipleValuesArrayFormatter>();

            It should_be_valid = () =>
                settings.IsValid.ShouldBeTrue();
        }

        class default_settings_in_general
        {
            It should_default_to_json_serializer = () =>
                settings.DefaultSerializer.ShouldBeOfExactType<DefaultJsonSerializer>();

            It should_have_json_deserializer = () =>
                settings.Serializers.ShouldContain(d => d is DefaultJsonSerializer);

            It should_be_valid = () =>
                settings.IsValid.ShouldBeTrue();
        }

        class without_array_formatter
        {
            Because of = () =>
                settings.ArrayFormatter = null;

            It should_not_be_valid = () =>
                settings.IsValid.ShouldBeFalse();
        }

        class when_customizing_serializer
        {
            static bool called;

            Because of = () =>
                settings.Configure<DefaultJsonSerializer>(s =>
                {
                    called = true;
                });

            It should_call_callback = () =>
                called.ShouldEqual(true);
        }

        class without_serializers
        {
            Because of = () =>
                settings.Serializers.Clear();

            It should_not_be_valid = () =>
                settings.IsValid.ShouldBeFalse();
        }

        class when_validating_invalid_settings
        {
            static Exception exception;

            Establish context = () =>
                settings.Serializers.Clear();

            Because of = () =>
                exception = Catch.Exception(() => settings.Validate());

            It should_throw = () =>
                exception.ShouldBeOfExactType<InvalidOperationException>();
        }

        class when_appending_middleware
        {
            Because of = () =>
                settings.Middleware.AppendMiddleware(new TestMiddleware());

            It should_have_middleware = () =>
                settings.Middleware.HasMiddleware<TestMiddleware>().ShouldBeTrue();

            //It should_add_to_end_of_middleware_chain = () =>
            //    settings.Middleware.Last().ShouldBeOfExactType<TestMiddleware>();
        }

        class when_prepending_middleware
        {
            Because of = () =>
                settings.Middleware.PrependMiddleware(new TestMiddleware());

            It should_have_middleware = () =>
                settings.Middleware.HasMiddleware<TestMiddleware>().ShouldBeTrue();

            //It should_add_to_start_of_middleware_chain = () =>
            //    settings.Middleware.First().ShouldBeOfExactType<TestMiddleware>();
        }

        class when_replacing_middleware_with_same_type
        {
            static UserAgentMiddleware replacement;

            Establish context = () =>
            {
                settings.Middleware.PrependMiddleware(new TestMiddleware());
                settings.Middleware.AppendMiddleware(new TestMiddleware());

                replacement = new UserAgentMiddleware();
            };

            Because of = () =>
                settings.Middleware.ReplaceMiddleware(replacement);

            //It should_replace_middleware_in_place = () =>
            //    settings.Middleware.ElementAt(1).ShouldBeOfExactType<UserAgentMiddleware>();

            It should_have_correct_middleware_count = () =>
                settings.Middleware.MiddlewareCount.ShouldEqual(3);

            //It should_replace_instance = () =>
            //    settings.Middleware.ElementAt(1).ShouldBeTheSameAs(replacement);
        }

        class TestMiddleware : IHttpMiddleware
        {
            public IHttpMiddleware Next { get; set; }

            public Task<IHttpResponse> Invoke(IHttpRequest request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
