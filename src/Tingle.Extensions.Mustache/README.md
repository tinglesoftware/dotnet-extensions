# Tingle.Extensions.Mustache

This library is based on [Mustache](https://github.com/ActiveCampaign/mustachio) by providing abstractions that help with creating dynamic content using templates.

## Template Syntax

Here is everything you need to know about the engine.

### Variables

The most basic mustache type is a variable. A `{{name}}` tag in a template will try to find the name key in the current context. If the name key is not found, nothing will be rendered.

If you have a template model like this (represented as JSON):

```json
{
  "user": {
    "name": "John"
  }
}
```

And this template:

`Welcome home {{ user.name }}`

The rendered output will be

`Welcome home John`

### Sections

Sections render blocks of text one or more times, depending on the value of the key in the current context.

A section begins with a pound and ends with a slash. That is, `{{#user}}` begins a "user" section while `{{/user}}` ends it.

#### Empty Lists or False Values

If the user key exists and has a value of false or an empty list, the HTML between the pound and slash will not be displayed.

Template:

```html
Shown.
{{#user}}
  Never shown!
{{/user}}
```

Data:

```json
{
   "user": false
}
```

Output:

`Never Shown!`

#### Non-Empty Lists

If the user key exists and has a non-false value, the HTML between the pound and slash will be rendered and displayed one or more times.

Template:

```html
{{#user}}
    <b>Welcome home {{ name }}</b>
{{/user}}
```

Data:

```json
{
  "user": [
    { "name": "John" },
    { "name": "Mary" }
  ]
}
```

Output:

```html
<b>Welcome home John</b>
<b>Welcome home Mary</b>
```

#### Inverted Sections

An inverted section begins with a caret (hat) and ends with a slash.

While sections can be used to render text one or more times based on the value of the key, inverted sections may render text once based on the inverse value of the key. That is, they will be rendered if the key doesn't exist, is false, or is an empty list.

Template:

```html
{{#repo}}
  <b></b>
{{/repo}}
{{^repo}}
  No repos!
{{/repo}}
```

Data:

```json
{

}
```

Output:

`No repos!`

#### Comments

Comments begin with a bang and are ignored. The following template:

```html
<h1>Today{{! ignore me }}.</h1>
```

Output:

```html
<h1>Today.</h1>
```

### Collections

Template:

```html
<p>{{name}} vehicles:</p>
<ul>
    {{#each vehicles}}
        <li>{{registration}}</li>
    {{/each}}
</ul>
```

Data:

```json
{
  "name": "John",
  "vehicles": [
    {
      "registration": "123 KL 3"
    },
    {
      "registration": "054F 87T"
    }
  ]
}
```

Output:

```html
<p>John vehicles:</p>
<ul>
    <li>123 KL 3</li>
    <li>054F 87T</li>
</ul>
```

#### Advanced Interpolation

If we've scoped out template to a property of a model, we may want to "reach up" to a property in the outer scope. Using the data in [Collections](#collections) example, we can use special interpolation syntax to do this without needing to repeat the values in our template model:

Template:

```html
<ul>
    {{#each vehicles}}
        <li>{{registration}} is owned by {{../../name}}</li>
    {{/each}}
</ul>
```

Output:

```html
<ul>
    <li>123 KL 3 is owned by John</li>
    <li>054F 87T is owned by John</li>
</ul>
```

Note the `../` in the template, which just means "go up one level" in my template model and look for the property name that follows. You can go up as many levels in you model as needed by repeating `../` multiple times at the start of you `{{ ... }}` section.

## How to use the library

The `MustacheTemplate` is the implementation for the [Mustache](https://github.com/ActiveCampaign/mustachio) based text templates. Below is an example of how to use it:

```cs
var template = new MustacheTemplate("Your OTP code is {{otp}} expires in {{minutes}} minutes.");
var values = new Dictionary<string, object?>
{
    ["otp"] = "12345",
    ["minutes"] = 59,
};
var rendered = template.Render(values);
```

The `rendered` result will be: `Your OTP code is 12345 expires in 59 minutes.`.

By default, the case isn't ignored when finding values to replace with. If you'd like to ignore the case you can set the `ignoreCase` parameter to `false` in the `MustacheTemplate`'s constructor.

By default, inference support us disabled. Inference allows for error detection, and faster debugging iterations when developing templates. To enable inference support you can set the `inference` parameter to `true` in the `MustacheTemplate`'s constructor.

You can also choose to read the text template from a file in the host environment the application is running in. You can use the `GetTemplate(...)` or `GetTemplateAsync(...)` extension methods for the `IHostEnvironment` to do this.

If you have access to a `Stream` you can use `MustacheTemplate`'s `Create(...)` or `CreateAsync(...)` to also accomplish this.
