# Design Direction: Product Catalog Manager

Reading this as: Product catalog backoffice for internal store managers, in a utilitarian ledger style, dial ENERGY 1 / RHYTHM 1 / MOTION 1.

Dial: ENERGY 1 / RHYTHM 1 / MOTION 1

## Visual Decisions and One-Line Reasons
- Why this palette: High-contrast slate and paper surfaces ensure readable item listings without visual fatigue.
- Why this layout: A single structured table and dialog form keep catalog navigation direct and free of unnecessary dashboard clutter.
- Why this typography: System sans-serif fonts guarantee crisp rendering and zero external font network latency across devices.
- Why this spacing: Compact 8px and 16px scales maximize visible table rows while retaining accessible touch targets.
- Why cards on mobile: Rows transform into stacked cards on narrow viewports to avoid horizontal scrolling.
- Why no decorative icons: Text labels communicate status and actions with complete clarity.

## Color Tokens (WCAG AA Compliant)

### Light Theme (Default)
- Surface: `#F7F7F8` (Paper background)
- Surface Panel: `#FFFFFF` (Card and table container)
- Border: `#D0D5DD` (Table lines and inputs, 3.2:1 against surface)
- Text Primary: `#111827` (Deep neutral, 15.8:1 against `#FFFFFF`)
- Text Secondary: `#4B5563` (Muted label, 7.0:1 against `#FFFFFF`)
- Brand Primary: `#0F766E` (Deep teal action button, 4.8:1 against `#FFFFFF`)
- Brand Primary Text: `#FFFFFF`
- Danger Action: `#B91C1C` (Delete confirmation, 5.8:1 against `#FFFFFF`)
- Focus Ring: `#0F766E` (2px visible outline, 3:1 minimum contrast)

### Dark Theme
- Surface: `#0F172A` (Deep navy slate background)
- Surface Panel: `#1E293B` (Elevated panel)
- Border: `#334155` (Subtle boundary, 3.1:1 against surface)
- Text Primary: `#F8FAFC` (Near-white neutral, 14.1:1 against `#1E293B`)
- Text Secondary: `#94A3B8` (Slate label, 5.3:1 against `#1E293B`)
- Brand Primary: `#14B8A6` (Bright teal action button, 4.6:1 against `#0F172A`)
- Brand Primary Text: `#0F172A`
- Danger Action: `#EF4444` (Delete button, 4.5:1 against `#0F172A`)
- Focus Ring: `#14B8A6` (2px visible outline)

## Mobile and Responsive Rules
- Semantic data table on viewports wider than 640px.
- Stacked item cards on viewports narrower than 640px with no horizontal scroll leak.
- Tap targets have a minimum height of 44px with 8px margin separation.
- Modal dialogs use `<dialog>` elements and close immediately upon pressing the Escape key.

## Tone and Copywriting
- Actions use plain, direct verbs: "Sign In", "Save Product", "Delete Product", "Apply Filter", "Reset Filter".
- Status notices explain state and next steps clearly without filler buzzwords.
