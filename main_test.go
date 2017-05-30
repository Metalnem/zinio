package main

import "testing"

func TestSanitize(t *testing.T) {
	tests := []struct {
		Input  string
		Output string
	}{
		{"Hello World", "Hello World"},
		{"Καλημέρα κόσμε", "Καλημέρα κόσμε"},
		{"こんにちは 世界", "こんにちは 世界"},
		{"Men's Health", "Men's Health"},
		{"act!ve", "act!ve"},
		{"Health & Fitness", "Health & Fitness"},
		{"Fat-Burning Manual", "Fat-Burning Manual"},
		{"Health Myths, Busted!", "Health Myths, Busted!"},
		{".NET Magazine", ".NET Magazine"},
		{"123 Easiest-Ever Recipes", "123 Easiest-Ever Recipes"},
		{"Something: Else", "Something - Else"},
		{"Something/Else", "Something - Else"},
		{"Something Else?", "Something Else"},
	}

	for _, test := range tests {
		want := test.Output
		got := sanitize(test.Input)

		if want != got {
			t.Errorf("want %s, got %s", want, got)
		}
	}
}
