// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     组合验证器
/// </summary>
public class CompositeValidator : ValidatorBase, IValidatorInitializer, IDisposable
{
    /// <summary>
    ///     <see cref="ValidatorBase" /> 集合
    /// </summary>
    internal readonly List<ValidatorBase> _validators;

    /// <summary>
    ///     高优先级验证器区域的结束索引
    /// </summary>
    /// <remarks>同时也是普通验证器区域的起始索引。</remarks>
    internal int _highPriorityEndIndex;

    /// <summary>
    ///     <inheritdoc cref="CompositeValidator" />
    /// </summary>
    /// <param name="validators">
    ///     <see cref="ValidatorBase" /> 列表
    /// </param>
    public CompositeValidator(params ValidatorBase[] validators)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validators);

        // 遍历验证器列表并添加
        _validators = [];
        foreach (var validator in validators)
        {
            AddValidatorCore(validator);
        }

        Validators = _validators;

        ErrorMessageResourceAccessor = () => null!;
    }

    /// <summary>
    ///     验证器列表
    /// </summary>
    public IReadOnlyList<ValidatorBase> Validators { get; }

    /// <summary>
    ///     <inheritdoc cref="CompositeMode" />
    /// </summary>
    /// <remarks>默认值为：<see cref="CompositeMode.FailFast" />。</remarks>
    public CompositeMode Mode { get; set; } = CompositeMode.FailFast;

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    void IValidatorInitializer.InitializeServiceProvider(Func<Type, object?>? serviceProvider) =>
        InitializeServiceProvider(serviceProvider);

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext) =>
        Mode switch
        {
            CompositeMode.FailFast or CompositeMode.All => Validators.All(u => u.IsValid(value, validationContext)),
            CompositeMode.Any => Validators.Any(u => u.IsValid(value, validationContext)),
            _ => throw new NotSupportedException()
        };

    /// <inheritdoc />
    public override List<ValidationResult>? GetValidationResults(object? value, IValidationContext? validationContext)
    {
        // 初始化验证结果集合
        var validationResults = new List<ValidationResult>();

        // 遍历验证器集合
        foreach (var validator in Validators)
        {
            // 获取对象验证结果集合
            if (validator.GetValidationResults(value, validationContext) is { Count: > 0 } results)
            {
                // 追加验证结果集合
                validationResults.AddRange(results);

                // 检查验证器模式是否是遇到首个验证失败即停止后续验证
                if (Mode is CompositeMode.FailFast)
                {
                    break;
                }
            }
            // 检查验证器模式是否是任一验证器验证成功，即视为整体验证通过
            else if (Mode is CompositeMode.Any)
            {
                // 清空验证结果集合
                validationResults.Clear();

                break;
            }
        }

        // 如果验证未通过且配置了自定义错误信息，则在首部添加自定义错误信息
        if (validationResults.Count > 0 && (string?)ErrorMessageString is not null)
        {
            validationResults.Insert(0,
                new ValidationResult(FormatErrorMessage(validationContext?.DisplayName!),
                    validationContext?.MemberNames));
        }

        return validationResults.ToResults();
    }

    /// <inheritdoc />
    public override void Validate(object? value, IValidationContext? validationContext)
    {
        // 初始化首个验证无效的验证器
        ValidatorBase? firstFailedValidator = null;

        // 遍历验证器集合
        foreach (var validator in Validators)
        {
            // 检查对象合法性
            if (!validator.IsValid(value, validationContext))
            {
                // 缓存首个验证无效的验证器
                firstFailedValidator ??= validator;

                // 检查验证器模式是否是遇到首个验证失败即停止后续验证
                if (Mode is CompositeMode.FailFast or CompositeMode.All)
                {
                    ThrowValidationException(value, validator, validationContext);
                }
            }
            // 检查验证器模式是否是任一验证器验证成功，即视为整体验证通过
            else if (Mode is CompositeMode.Any)
            {
                return;
            }
        }

        // 空检查
        if (firstFailedValidator is not null)
        {
            ThrowValidationException(value, firstFailedValidator, validationContext);
        }
    }

    /// <inheritdoc />
    public override string? FormatErrorMessage(string name) =>
        (string?)ErrorMessageString is null ? null : base.FormatErrorMessage(name);

    /// <summary>
    ///     添加验证器
    /// </summary>
    /// <param name="validators">
    ///     <see cref="ValidatorBase" /> 列表
    /// </param>
    /// <returns>
    ///     <see cref="CompositeValidator" />
    /// </returns>
    public CompositeValidator Add(params ValidatorBase[] validators)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validators);

        // 遍历验证器列表并添加
        foreach (var validator in validators)
        {
            AddValidatorCore(validator);
        }

        return this;
    }

    /// <summary>
    ///     设置验证模式
    /// </summary>
    /// <param name="mode">
    ///     <see cref="CompositeMode" />
    /// </param>
    /// <returns>
    ///     <see cref="CompositeValidator" />
    /// </returns>
    public CompositeValidator UseMode(CompositeMode mode)
    {
        Mode = mode;

        return this;
    }

    /// <summary>
    ///     抛出验证异常
    /// </summary>
    /// <param name="value">对象</param>
    /// <param name="validator">
    ///     <see cref="ValidatorBase" />
    /// </param>
    /// <param name="validationContext">
    ///     <see cref="IValidationContext" />
    /// </param>
    /// <exception cref="ValidationException"></exception>
    internal void ThrowValidationException(object? value, ValidatorBase validator,
        IValidationContext? validationContext)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validator);

        // 检查是否配置了自定义错误信息
        if ((string?)ErrorMessageString is null)
        {
            validator.Validate(value, validationContext);
        }
        else
        {
            throw new ValidationException(
                new ValidationResult(FormatErrorMessage(validationContext?.DisplayName!),
                    validationContext?.MemberNames), null, value);
        }
    }

    /// <summary>
    ///     释放资源
    /// </summary>
    /// <param name="disposing">是否释放托管资源</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        // 释放所有验证器资源
        foreach (var validator in Validators)
        {
            if (validator is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    /// <summary>
    ///     添加验证器内部方法
    /// </summary>
    /// <param name="validator">
    ///     <see cref="ValidatorBase" />
    /// </param>
    internal void AddValidatorCore(ValidatorBase validator)
    {
        // 空检查 
        ArgumentNullException.ThrowIfNull(validator);

        // 检查是否是高优先级验证器
        if (validator is IHighPriorityValidator highPriorityValidator)
        {
            // 只在 [0, _highPriorityEndIndex) 范围内查找插入位置（保持 Priority 升序）
            var insertIndex = _highPriorityEndIndex;
            for (var i = 0; i < _highPriorityEndIndex; i++)
            {
                // ReSharper disable once InvertIf
                if (_validators[i] is IHighPriorityValidator existing &&
                    existing.Priority > highPriorityValidator.Priority)
                {
                    insertIndex = i;
                    break;
                }
            }

            _validators.Insert(insertIndex, validator);

            // 高优先级区域扩大
            _highPriorityEndIndex++;
        }
        else
        {
            _validators.Add(validator);
        }
    }

    /// <inheritdoc cref="IValidatorInitializer.InitializeServiceProvider" />
    internal void InitializeServiceProvider(Func<Type, object?>? serviceProvider)
    {
        // 遍历所有验证器并尝试同步 IServiceProvider 委托
        foreach (var validator in Validators)
        {
            // 检查验证器是否实现 IValidatorInitializer 接口
            if (validator is IValidatorInitializer initializer)
            {
                // 同步 IServiceProvider 委托
                initializer.InitializeServiceProvider(serviceProvider);
            }
        }
    }
}