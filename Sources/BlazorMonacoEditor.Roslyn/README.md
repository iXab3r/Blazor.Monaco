# Architecture
Editors - each has unique Id generated when created
MonacoInterop - singleton, holds N editors, can retrieve them by Id

### How to register code completion/action provider
monaco.languages.registerCompletionItemProvider
monaco.languages.registerCodeActionProvider

