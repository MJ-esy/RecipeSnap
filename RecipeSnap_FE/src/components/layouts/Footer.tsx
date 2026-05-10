const currentYear = new Date().getFullYear()

export default function Footer() {
    return (
        <footer style={{
            borderTop: '1px solid var(--border)',
            padding: '24px 32px',
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            flexWrap: 'wrap',
            gap: '12px',
            fontSize: '14px',
            color: 'var(--text)',
        }}>
            <span style={{ color: 'var(--text-h)', fontWeight: 500 }}>
                RecipeSnap
            </span>

            <span>© {currentYear} RecipeSnap</span>
        </footer>
    )
}
