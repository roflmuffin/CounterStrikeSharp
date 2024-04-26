# Contributing to CounterStrikeSharp

We'd love for you to contribute to CS# to make it better than it is today!

Here are the guidelines we'd like you to follow:

 - [Question or Problem?](#question)
 - [Issues and Bugs](#issue)
 - [Submission Guidelines](#submit)
 - [Coding Format](#format)

## <a name="question"></a> Got a Question or Problem?

If you have questions about how to contribute to CounterStrikeSharp, please join our [Discord][discord] server.

## <a name="issue"></a> Found an Issue?

If you find a bug in the source code or a mistake in the documentation, you can help us by
submitting an issue to our [GitHub Repository][github]. Even better you can submit a Pull Request
with a fix.

**Please see the [Submission Guidelines](#submit) below.**

## <a name="submit"></a> Submission Guidelines

### Submitting an Issue
Before you submit your issue please search the archive, maybe your question was already answered.

If your issue appears to be a bug and hasn't been reported, open a new issue. Help us to maximize
the effort we can spend fixing issues and adding new features, by not reporting duplicate issues.
Providing the following information will increase the chances of your issue being dealt with
quickly:

* **Overview of the Issue** - if an error is being thrown a stack trace helps
* **Operating System** - is this a problem with a specific OS (Windows or Linux)?
* **Reproduce the Error** - provide details, if possible, on how to reproduce the error
* **Related Issues** - has a similar issue been reported before?
* **Suggest a Fix** - if you can't fix the bug yourself, perhaps you can point to what might be causing the problem (line of code or commit)

### Submitting a Pull Request
Before you submit your pull request consider the following guidelines:

* Search [GitHub](https://github.com/roflmuffin/CounterStrikeSharp/pulls) for an open or closed Pull Request
  that relates to your submission. You don't want to duplicate effort.
* If adding a feature or enhancement, we recommend you first [start a discussion for
  it](https://github.com/roflmuffin/CounterStrikeSharp/discussions) before submitting a Pull Request.
* [Fork](https://help.github.com/articles/fork-a-repo/) this repo.
* [Clone](https://help.github.com/articles/cloning-a-repository/) your copy.
    ```shell
    git clone https://github.com/YOUR_USERNAME/CounterStrikeSharp.git
    cd CounterStrikeSharp/
    ```
* After cloning, set a new remote [upstream](https://help.github.com/articles/configuring-a-remote-for-a-fork/) (this helps to keep your fork up to date)
    
    ```shell
    git remote add upstream https://github.com/roflmuffin/CounterStrikeSharp.git
    ```

* Make your changes in a new git branch:

    ```shell
    git checkout -b my-fix-branch master
    ```

* Create your patch and run appropriate tests.
* Commit your changes using a descriptive commit message that uses the imperative, present tense: "change" not "changed" nor "changes".

    ```shell
    git commit -a
    ```
  Note: the optional commit `-a` command line option will automatically "add" and "rm" edited files.

* Push your branch to GitHub:

    ```shell
    git push origin my-fix-branch
    ```

In GitHub, send a pull request to `CounterStrikeSharp:master`.
If we suggest changes, then:

* Make the required updates.
* Re-run CounterStrikeSharp to ensure everything is still working & tests are passing.
* Commit your changes to your branch (e.g. `my-fix-branch`).
* Push the changes to your GitHub repository (this will update your Pull Request).

If the PR gets too outdated we may ask you to rebase and force push to update the PR:

```shell
git fetch upstream
git rebase upstream/master
git push origin my-fix-branch -f
```

That's it! Thank you for your contribution!

#### After your pull request is merged

After your pull request is merged, you can safely delete your branch and pull the changes
from the main (upstream) repository.

[github]: https://github.com/roflmuffin/CounterStrikeSharp
[discord]: https://discord.gg/eAZU3guKWU