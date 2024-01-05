# Architecture
## Interop and general stuff
- Monaco dependencies and code are about ~10MB in total, meaning that if we want to use it effectively
  it has to be loaded only once (i.e. no JS/CSS isolation) => Singleton on JS side 
- Singleton on JS side is in MonacoInterop.ts, on C# in MonacoInterop.cs (scoped dependency)
- TextModels are represented by URIs, which are immutable. It is also possible to reference models by ID, but it 
  does not make a lot of sense considering URI is unique enough and we're not expecting to have multiple copies of 
  the same file opened (source code splicing will need it though)
- IDs are not unique across editors, they look something like '$model1'

## AutoComplete
- registerCompletionProvider in Monaco is a singleton, i.e. it is expected 
  that completion provider can serve any model loaded into any editor, meaning that on top-level it also has to be singleton
- 