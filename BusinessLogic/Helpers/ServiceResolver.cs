using Contracts.Interfaces;

namespace BusinessLogic.Helpers;

public delegate IWordRepository ServiceResolver(RepositoryType key);