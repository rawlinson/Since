# Avers 0.2.0

A Versioning System based on [Semantic Versioning](http://semver.org/) 2.0.0 with altered extensions.

> **Verb**

> **aver** /əˈvɜː/ ‎(_third-person singular simple present [**avers**](https://en.wiktionary.org/wiki/avers#English), present participle **averring**, simple past and past participle **averred**_)

> 1. _to assert the truth of, to affirm with confidence; to declare in a positive manner._

## Summary

Given a version number `<major>.<minor>.<patch>`, increment the:

1. `<major>` version when you make incompatible API changes,
2. `<minor>` version when you add functionality in a backwards-compatible manner, and
3. `<patch>` version when you make backwards-compatible bug fixes.

Additional labels and build metadata are available as extensions to the `<major>.<minor>.<patch>` format.

## Extensions

The following extensions are included and should appear in the order shown:

1. `_<tag string>` specifies the tag of the build and must always match.
2. `-<pre-release version>` specifies the pre-release version of the build and is considered to be before the main version.
3. `+<post-release version>` specifies the post-release version of the build and is considered to be after the main version.
4. `␣(<build string>)` specifies the build string and is always ignored.

## Examples

* `1.2.3_hotfix.b234+dev.2342 (g234534)`
