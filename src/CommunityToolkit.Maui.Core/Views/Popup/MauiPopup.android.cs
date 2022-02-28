﻿using System;
using System.Diagnostics.CodeAnalysis;
using Android.App;
using Android.Content;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Platform;
using AView = Android.Views.View;

namespace CommunityToolkit.Core.Views;

/// <summary>
/// The navite implementation of Popup control.
/// </summary>
public class MauiPopup : Dialog, IDialogInterfaceOnCancelListener
{
	readonly IMauiContext mauiContext;

	/// <summary>
	/// Constructor of <see cref="MauiPopup"/>.
	/// </summary>
	/// <param name="context">An instance of <see cref="Context"/>.</param>
	/// <param name="mauiContext">An instace of <see cref="IMauiContext"/>.</param>
	/// <exception cref="ArgumentNullException">If <paramref name="mauiContext"/> is null an exception will be thrown. </exception>
	public MauiPopup(Context context, IMauiContext mauiContext)
		: base(context)
	{
		this.mauiContext = mauiContext ?? throw new ArgumentNullException(nameof(mauiContext));
	}

	/// <summary>
	/// An instace of the <see cref="IPopup"/>.
	/// </summary>
	public IPopup? VirtualView { get; private set; }

	/// <summary>
	/// Method to initialize the native implementation.
	/// </summary>
	/// <param name="element">An instance of <see cref="IPopup"/>.</param>
	public AView? SetElement(IPopup? element)
	{
		ArgumentNullException.ThrowIfNull(element);

		VirtualView = element;

		if (TryCreateContainer(VirtualView, out var container))
		{
			SubscribeEvents();
		}

		return container;
	}

	/// <summary>
	/// Method to show the Popup.
	/// </summary>
	public override void Show()
	{
		base.Show();

		_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} cannot be null");

		VirtualView.OnOpened();
	}

	/// <summary>
	/// Method triggered when the Popup is LightDismissed.
	/// </summary>
	/// <param name="dialog">An instance of the <see cref="IDialogInterface"/>.</param>
	public void OnCancel(IDialogInterface? dialog)
	{
		_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} cannot be null");
		_ = VirtualView.Handler ?? throw new InvalidOperationException($"{nameof(VirtualView.Handler)} cannot be null");

		VirtualView.Handler.Invoke(nameof(IPopup.LightDismiss));
	}

	/// <summary>
	/// Method to CleanUp the resources of the <see cref="MauiPopup"/>.
	/// </summary>
	public void CleanUp()
	{
		VirtualView = null;
	}

	bool TryCreateContainer(in IPopup basePopup, [NotNullWhen(true)] out AView? container)
	{
		container = null;

		if (basePopup.Content is null)
		{
			return false;
		}

		container = basePopup.Content.ToNative(mauiContext);
		SetContentView(container);

		return true;
	}

	void SubscribeEvents()
	{
		SetOnCancelListener(this);
	}
}
